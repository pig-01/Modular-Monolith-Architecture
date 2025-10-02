using Base.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace DataHub.Infrastructure.Services;

public class IdentityService(
    ILogger<IdentityService> logger) : BaseService, IIdentityService
{
    public Task<string> GetUserIdAsync(string customerId, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<string> GetUserNameAsync(string userId, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsUserInRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
