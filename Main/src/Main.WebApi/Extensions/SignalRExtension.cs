using Base.Infrastructure.Interface.Progress;
using Main.WebApi.Application.Hubs;
using Main.WebApi.Infrastructure;

namespace Main.WebApi.Extensions;

public static class SignalRExtension
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR(options => options.EnableDetailedErrors = true);
        services.AddScoped<IBatchProgressService, BatchProgressService>();
        return services;
    }

    public static WebApplication MapSignalRHubs(this WebApplication app)
    {
        app.MapHub<BatchHub>("/batchHub");
        return app;
    }
}