using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.Application.MultiTenant;
using User.Application.Pipeline;
using User.Infrastructure;
using User.Infrastructure.MultiTenant;

namespace User.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUserModule(this IServiceCollection services, IConfiguration configuration)
    {
        // --- Multi-tenant context factory ---
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
        services.Configure<TenantOptions>(configuration.GetSection(TenantOptions.SectionName));
        services.AddScoped<ITenantDbContextFactory<UserDbContext>, TenantUserDbContextFactory>();

        // Register UserDbContext by delegating to the factory so the connection string
        // resolution logic lives in exactly one place (TenantUserDbContextFactory).
        services.AddScoped<UserDbContext>(serviceProvider =>
            serviceProvider.GetRequiredService<ITenantDbContextFactory<UserDbContext>>()
                           .CreateDbContext());

        // --- CQRS pipeline ---
        services.AddMediatR(typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}
