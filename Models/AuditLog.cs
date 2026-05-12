using System;

namespace Compliance_Hub.api.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Action { get; set; }
        public required string EntityType { get; set; }
        public int EntityId { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
