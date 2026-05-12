using System;
using System.Collections.Generic;

namespace Compliance_Hub.api.Models
{
    public class Department
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<User> Users { get; set; } = new List<User>();
        public List<Document> Documents { get; set; } = new List<Document>();
    }
}
