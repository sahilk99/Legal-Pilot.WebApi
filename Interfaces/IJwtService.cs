using Legal_Pilot.api.Models;

namespace Legal_Pilot.api.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
