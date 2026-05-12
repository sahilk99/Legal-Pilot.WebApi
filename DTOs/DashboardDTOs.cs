using System;
using System.Collections.Generic;

namespace Compliance_Hub.api.DTOs
{
    public class DashboardSummaryDTO
    {
        public int TotalDocuments { get; set; }
        public int CompliantCount { get; set; }
        public int ExpiringSoonCount { get; set; }
        public int OverdueCount { get; set; }
        public double OverallCompliancePercentage { get; set; }
        public List<DepartmentStatDTO> DepartmentStats { get; set; } = new List<DepartmentStatDTO>();
        public List<RecentActivityDTO> RecentActivity { get; set; } = new List<RecentActivityDTO>();
    }

    public class DepartmentStatDTO
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalDocuments { get; set; }
        public int CompliantCount { get; set; }
        public double CompliancePercentage { get; set; }
    }

    public class RecentActivityDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class ExpiringSoonDocumentDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
