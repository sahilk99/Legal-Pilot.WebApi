using System;
using System.Collections.Generic;

namespace Compliance_Hub.api.Models
{
    public class Document
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Category { get; set; } 
        public string? FileUrl { get; set; }
        public int UploadedById { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public required string Status { get; set; } 
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User? Uploader { get; set; }
        public Department? Department { get; set; }
    }
}
