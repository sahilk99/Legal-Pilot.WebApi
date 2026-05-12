using System;

namespace Compliance_Hub.api.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? DocumentId { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Document? Document { get; set; }
    }
}
