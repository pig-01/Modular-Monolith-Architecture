namespace User.Infrastructure.MultiTenant;

/// <summary>
/// Provides the current tenant identifier, used to select the appropriate
/// database connection string for the logged-in user.
/// </summary>
public interface ITenantProvider
{
    string GetTenantId();
}
