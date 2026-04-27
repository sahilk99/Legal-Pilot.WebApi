namespace Legal_Pilot.api.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Department? Department { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
