using DataHub.Domain.AggregatesModel.UserAggregate;
using DataHub.Infrastructure.Contexts;

namespace DataHub.Infrastructure.Repositories;

public class UserRepository(DemoContext context) : IUserRepository
{
    public Task<Scuser> AddAsync(Scuser entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task DeleteAsync(Scuser entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Scuser> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<IEnumerable<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task UpdateAsync(Scuser entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
