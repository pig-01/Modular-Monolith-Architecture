using Scheduler.Extensions;
using Serilog;

try
{
    WebApplication.CreateBuilder(args)
        .AddApplicationService()
        .Build()
        .UseApplicationMiddleware()
        .MapDefaultEndpoints()
        .MapSchedulerEndpoints()
        .UseDefaultOpenApi()
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Web application stopped");
    Log.CloseAndFlush();
}

public partial class Program { }