using Base.Infrastructure.Services;

namespace DataHub.Infrastructure.Services;

public interface IIdentityService : IService
{
    Task<string> GetUserIdAsync(string customerId, CancellationToken cancellationToken = default);
    Task<string> GetUserNameAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserInRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);
}
