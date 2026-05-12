using System;
using System.Threading.Tasks;
using Compliance_Hub.api.Data;
using Compliance_Hub.api.Interfaces;
using Compliance_Hub.api.Models;

namespace Compliance_Hub.api.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int userId, string action, string entityType, int entityId, string description)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = description,
                    Timestamp = DateTime.UtcNow
                };

                await _context.AuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }
    }
}
