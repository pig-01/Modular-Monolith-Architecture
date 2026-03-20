using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace User.Infrastructure.MultiTenant;

/// <summary>
/// Creates a <see cref="UserDbContext"/> whose connection string is resolved at
/// runtime from the current tenant identifier supplied by <see cref="ITenantProvider"/>.
///
/// Resolution order:
///   1. Per-tenant entry in <see cref="TenantOptions.ConnectionStrings"/>
///   2. "DefaultConnection" from <see cref="IConfiguration"/>
/// </summary>
public class TenantUserDbContextFactory : ITenantDbContextFactory<UserDbContext>
{
    private readonly ITenantProvider _tenantProvider;
    private readonly TenantOptions _tenantOptions;
    private readonly string _defaultConnectionString;
    private readonly Func<string, DbContextOptions<UserDbContext>> _buildOptions;

    public TenantUserDbContextFactory(
        ITenantProvider tenantProvider,
        IOptions<TenantOptions> tenantOptions,
        IConfiguration configuration)
        : this(tenantProvider, tenantOptions, configuration,
               cs => new DbContextOptionsBuilder<UserDbContext>()
                         .UseSqlServer(cs)
                         .Options)
    {
    }

    /// <summary>
    /// Constructor that accepts a custom <paramref name="buildOptions"/> delegate,
    /// allowing an alternative DB provider (e.g., in-memory for tests).
    /// </summary>
    public TenantUserDbContextFactory(
        ITenantProvider tenantProvider,
        IOptions<TenantOptions> tenantOptions,
        IConfiguration configuration,
        Func<string, DbContextOptions<UserDbContext>> buildOptions)
    {
        _tenantProvider = tenantProvider;
        _tenantOptions = tenantOptions.Value;
        _defaultConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        _buildOptions = buildOptions;
    }

    public UserDbContext CreateDbContext()
    {
        var tenantId = _tenantProvider.GetTenantId();

        var connectionString = _tenantOptions.ConnectionStrings.TryGetValue(tenantId, out var cs)
            ? cs
            : _defaultConnectionString;

        return new UserDbContext(_buildOptions(connectionString));
    }
}
