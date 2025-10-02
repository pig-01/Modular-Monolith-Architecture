namespace Scheduler.Domain.AggregateModel.UserAggregate;

public interface IUserRepository : IRepository<Scuser>
{
    Task SwitchCurrentCulture(string userId, string culture, CancellationToken cancellationToken = default);

    Task SwitchCurrentTenant(string userId, string tenantId, CancellationToken cancellationToken = default);

    Task ModifyUserRole(string userId, string tenantId, long roleId, CancellationToken cancellationToken = default);

    Task AddSCUserTenantAsync(ScuserTenant scuserTenant, CancellationToken cancellationToken = default);

    Task<int> ActivateUserAsync(string userId, string tenantId, string token, DateTime activatedAt, CancellationToken cancellationToken = default);
}
