using DataHub.Domain.SeedWork;

namespace DataHub.Domain.AggregatesModel.UserAggregate;

public interface IUserRepository : IRepository<Scuser>
{
    Task<Scuser> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
}
