using Microsoft.EntityFrameworkCore;
using Compliance_Hub.api.Data;
using Compliance_Hub.api.DTOs;
using Compliance_Hub.api.Interfaces;
using Compliance_Hub.api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compliance_Hub.api.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _context;

        public DocumentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DocumentResponseDTO>> GetAllAsync(string? category, string? status, int? departmentId)
        {
            var query = _context.Documents
                .Include(d => d.Department)
                .Include(d => d.Uploader)
                .Where(d => !d.IsDeleted);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(d => d.Category == category);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.Status == status);

            if (departmentId.HasValue)
                query = query.Where(d => d.DepartmentId == departmentId);

            return await query.Select(d => new DocumentResponseDTO
            {
                Id = d.Id,
                Title = d.Title,
                Category = d.Category,
                FileUrl = d.FileUrl,
                UploadedById = d.UploadedById,
                UploadedByName = d.Uploader != null ? d.Uploader.Name : null,
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Department != null ? d.Department.Name : null,
                ExpiryDate = d.ExpiryDate,
                Status = d.Status,
                IsDeleted = d.IsDeleted,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            }).ToListAsync();
        }

        public async Task<DocumentResponseDTO?> GetByIdAsync(int id)
        {
            var d = await _context.Documents
                .Include(d => d.Department)
                .Include(d => d.Uploader)
                .FirstOrDefaultAsync(doc => doc.Id == id && !doc.IsDeleted);

            if (d == null) return null;

            return new DocumentResponseDTO
            {
                Id = d.Id,
                Title = d.Title,
                Category = d.Category,
                FileUrl = d.FileUrl,
                UploadedById = d.UploadedById,
                UploadedByName = d.Uploader != null ? d.Uploader.Name : null,
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Department != null ? d.Department.Name : null,
                ExpiryDate = d.ExpiryDate,
                Status = d.Status,
                IsDeleted = d.IsDeleted,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            };
        }

        public async Task<DocumentResponseDTO> CreateAsync(CreateDocumentDTO dto, int uploadedById)
        {
            var document = new Document
            {
                Title = dto.Title,
                Category = dto.Category,
                FileUrl = dto.FileUrl,
                DepartmentId = dto.DepartmentId,
                ExpiryDate = dto.ExpiryDate,
                UploadedById = uploadedById,
                Status = "Compliant"
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(document.Id))!;
        }

        public async Task<DocumentResponseDTO?> UpdateAsync(int id, UpdateDocumentDTO dto)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null || document.IsDeleted) return null;

            if (dto.Title != null) document.Title = dto.Title;
            if (dto.Category != null) document.Category = dto.Category;
            if (dto.FileUrl != null) document.FileUrl = dto.FileUrl;
            if (dto.ExpiryDate != null) document.ExpiryDate = dto.ExpiryDate;
            if (dto.Status != null) document.Status = dto.Status;

            document.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(document.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null || document.IsDeleted) return false;

            document.IsDeleted = true;
            document.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
