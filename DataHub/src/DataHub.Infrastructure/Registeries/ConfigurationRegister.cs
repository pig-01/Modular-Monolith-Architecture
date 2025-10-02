using DataHub.Infrastructure.Filters;
using DataHub.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataHub.Infrastructure.Registeries;

public static class ConfigurationRegister
{
    public static IServiceCollection DataHubConfigure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DataHubOptions>(configuration.GetSection(DataHubOptions.Position));
        services.Configure<BizformOptions>(configuration.GetSection(BizformOptions.Position));
        services.Configure<OrderOptions>(configuration.GetSection(OrderOptions.Position));
        services.Configure<RadarOptions>(configuration.GetSection(RadarOptions.Position));

        services.AddScoped<DemoAuthorizationFilter>();

        return services;
    }
}
