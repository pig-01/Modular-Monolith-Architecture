using System.Security.Claims;
using Base.Domain.Models.Authentication;

namespace Base.Infrastructure.Interface.Authentication;

public interface IJwtGenerateService : IDisposable
{
    void AddRole(string role);
    void AddRoles(IEnumerable<string> roles);
    void ClearRoles();
    List<string> GetRoles();
    JwtToken GenerateToken(string userId, string userName, Guid jwtId, int expireMinutes = 30);
    JwtToken GenerateToken(string userId, string userName, string userEmail, Guid jwtId, int expireMinutes = 30);
    JwtToken GenerateRefreshToken(string userId, string userName, Guid jwtId, int expireMinutes = 60);
    JwtToken GenerateRefreshToken(string userId, string userName, string userEmail, Guid jwtId, int expireMinutes = 60);
    Task<ClaimsPrincipal?> GetPrincipalAsync(string accessToken);
}
