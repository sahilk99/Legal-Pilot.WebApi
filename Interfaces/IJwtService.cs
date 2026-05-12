using Compliance_Hub.api.Models;

namespace Compliance_Hub.api.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
