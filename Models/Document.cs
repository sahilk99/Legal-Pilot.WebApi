namespace Legal_Pilot.api.Models
{
    public class Document
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Category { get; set; }
        public required string FileUrl { get; set; }
        public int UploadedBy { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? Uploader { get; set; }
        public Department? Department { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
