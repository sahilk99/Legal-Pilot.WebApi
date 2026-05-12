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
    [Route("api/audit-logs")]
    [Authorize(Roles = "Admin,Auditor")]
    public class AuditLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditLogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? userId,
            [FromQuery] string? entityType,
            [FromQuery] string? action,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // Pagination limits
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;

            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            // Filters
            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId);
            }

            if (!string.IsNullOrEmpty(entityType))
            {
                query = query.Where(a => a.EntityType == entityType);
            }

            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(a => a.Action == action);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= toDate.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditLogResponseDTO
                {
                    Id = a.Id,
                    UserName = a.User != null ? a.User.Name : null,
                    UserEmail = a.User != null ? a.User.Email : null,
                    Action = a.Action,
                    EntityType = a.EntityType,
                    EntityId = a.EntityId,
                    Description = a.Description,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();

            return Ok(new PaginatedAuditLogResponse
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            });
        }

        [HttpGet("document/{documentId}")]
        public async Task<IActionResult> GetDocumentHistory(int documentId)
        {
            var history = await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.EntityType == "Document" && a.EntityId == documentId)
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new AuditLogResponseDTO
                {
                    Id = a.Id,
                    UserName = a.User != null ? a.User.Name : null,
                    UserEmail = a.User != null ? a.User.Email : null,
                    Action = a.Action,
                    EntityType = a.EntityType,
                    EntityId = a.EntityId,
                    Description = a.Description,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}
