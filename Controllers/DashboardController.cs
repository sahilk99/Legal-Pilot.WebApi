using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Compliance_Hub.api.Data;
using Compliance_Hub.api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compliance_Hub.api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        [Authorize(Roles = "Admin,Auditor")]
        public async Task<IActionResult> GetSummary()
        {
            // Optimization: Fetching all dashboard data in a single round-trip by projecting multiple 
            // sub-queries into a single anonymous object.
            var dashboardData = await _context.Departments
                .Select(d => 1) // Dummy select to start the chain
                .Take(1)
                .Select(dummy => new
                {
                    DeptData = _context.Departments.Select(dep => new
                    {
                        dep.Name,
                        Total = dep.Documents.Count(d => !d.IsDeleted),
                        Compliant = dep.Documents.Count(d => !d.IsDeleted && d.Status == "Compliant"),
                        Expiring = dep.Documents.Count(d => !d.IsDeleted && d.Status == "ExpiringSoon"),
                        Overdue = dep.Documents.Count(d => !d.IsDeleted && d.Status == "Overdue")
                    }).ToList(),

                    RecentLogs = _context.AuditLogs
                        .OrderByDescending(a => a.Timestamp)
                        .Take(10)
                        .Select(a => new RecentActivityDTO
                        {
                            UserName = a.User != null ? a.User.Name : "System",
                            Action = a.Action,
                            EntityType = a.EntityType,
                            Description = a.Description ?? string.Empty,
                            Timestamp = a.Timestamp
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (dashboardData == null)
            {
                return Ok(new DashboardSummaryDTO());
            }

            var summary = new DashboardSummaryDTO
            {
                TotalDocuments = dashboardData.DeptData.Sum(d => d.Total),
                CompliantCount = dashboardData.DeptData.Sum(d => d.Compliant),
                ExpiringSoonCount = dashboardData.DeptData.Sum(d => d.Expiring),
                OverdueCount = dashboardData.DeptData.Sum(d => d.Overdue),
                DepartmentStats = dashboardData.DeptData.Select(d => new DepartmentStatDTO
                {
                    DepartmentName = d.Name,
                    TotalDocuments = d.Total,
                    CompliantCount = d.Compliant,
                    CompliancePercentage = d.Total > 0 ? (double)d.Compliant / d.Total * 100 : 0
                }).ToList(),
                RecentActivity = dashboardData.RecentLogs
            };

            summary.OverallCompliancePercentage = summary.TotalDocuments > 0 
                ? (double)summary.CompliantCount / summary.TotalDocuments * 100 
                : 0;

            return Ok(summary);
        }

        [HttpGet("expiring-soon")]
        [Authorize(Roles = "Admin,Auditor,Viewer")]
        public async Task<IActionResult> GetExpiringSoon()
        {
            var now = DateTime.UtcNow;
            var thirtyDaysFromNow = now.AddDays(30);

            var documents = await _context.Documents
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted && d.ExpiryDate != null && d.ExpiryDate <= thirtyDaysFromNow && d.ExpiryDate >= now)
                .OrderBy(d => d.ExpiryDate)
                .Select(d => new ExpiringSoonDocumentDTO
                {
                    Id = d.Id,
                    Title = d.Title,
                    Category = d.Category,
                    DepartmentName = d.Department != null ? d.Department.Name : "Unassigned",
                    ExpiryDate = d.ExpiryDate,
                    Status = d.Status
                })
                .ToListAsync();

            return Ok(documents);
        }
    }
}
