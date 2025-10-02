using Main.WebApi.Extensions;

try
{
    WebApplication.CreateBuilder(args)
        .AddApplicationService()
        .Build()
        .UseApplicationMiddleware()
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }