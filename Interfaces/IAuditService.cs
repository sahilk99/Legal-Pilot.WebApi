using System.Threading.Tasks;

namespace Compliance_Hub.api.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int userId, string action, string entityType, int entityId, string description);
    }
}
