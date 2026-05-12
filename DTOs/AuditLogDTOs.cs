using System;

namespace Compliance_Hub.api.DTOs
{
    public class AuditLogResponseDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PaginatedAuditLogResponse
    {
        public List<AuditLogResponseDTO> Data { get; set; } = new List<AuditLogResponseDTO>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
