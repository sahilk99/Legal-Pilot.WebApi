using System.Collections.Generic;
using System.Threading.Tasks;
using Compliance_Hub.api.DTOs;

namespace Compliance_Hub.api.Interfaces
{
    public interface IDocumentService
    {
        Task<List<DocumentResponseDTO>> GetAllAsync(string? category, string? status, int? departmentId);
        Task<DocumentResponseDTO?> GetByIdAsync(int id);
        Task<DocumentResponseDTO> CreateAsync(CreateDocumentDTO dto, int uploadedById);
        Task<DocumentResponseDTO?> UpdateAsync(int id, UpdateDocumentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
