using System;
using System.Threading.Tasks;
using Legal_Pilot.api.Data;
using Legal_Pilot.api.Interfaces;
using Legal_Pilot.api.Models;

namespace Legal_Pilot.api.Services
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
