using DataHub.Cloud.Application.Queries;
using DataHub.Cloud.Repositories;

namespace DataHub.Cloud.Extensions;

public static class Extension
{
    public static IServiceCollection AddCloudService(this IServiceCollection services)
    {
        services.AddScoped<IProvisionRepository, ProvisionRepository>();
        services.AddScoped<IOrderQuery, OrderQuery>();

        return services;
    }
}
