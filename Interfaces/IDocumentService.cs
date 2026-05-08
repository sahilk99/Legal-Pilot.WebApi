using System.Collections.Generic;
using System.Threading.Tasks;
using Legal_Pilot.api.DTOs;

namespace Legal_Pilot.api.Interfaces
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
