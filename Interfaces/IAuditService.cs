using System.Threading.Tasks;

namespace Legal_Pilot.api.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int userId, string action, string entityType, int entityId, string description);
    }
}
