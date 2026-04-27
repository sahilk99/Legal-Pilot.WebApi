namespace Legal_Pilot.api.Models
{
    public class Department
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
