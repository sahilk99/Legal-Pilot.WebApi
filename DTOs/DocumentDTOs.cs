using System;

namespace Compliance_Hub.api.DTOs
{
    public class CreateDocumentDTO
    {
        public required string Title { get; set; }
        public string? Category { get; set; }
        public string? FileUrl { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class UpdateDocumentDTO
    {
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
    }

    public class DocumentResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? FileUrl { get; set; }
        public int UploadedById { get; set; }
        public string? UploadedByName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
