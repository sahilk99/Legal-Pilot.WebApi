namespace Legal_Pilot.api.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Action { get; set; }
        public required string EntityType { get; set; }
        public int EntityId { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? User { get; set; }
    }
}
