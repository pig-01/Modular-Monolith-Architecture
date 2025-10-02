using Aspire.ServiceDefaults;
using DataHub.Bizform.Endpoints;
using DataHub.Cloud.Endpoints;
using DataHub.Extensions;
using Serilog;

try
{
    Log.Information("Web application starting up");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    Log.Information("Web application services added to the container");
    builder.AddApplicationServices();

    WebApplication app = builder.Build();

    // Redirect HTTP to HTTPS
    _ = app.UseHttpsRedirection().UseHsts();

    // Configure the HTTP request pipeline.
    app.AddDapperTypeHandler();
    _ = app.UseExceptionHandler();

    // Configure the HTTP request pipeline.
    Log.Information("Web application endpoints mapped");
    _ = app.MapDefaultEndpoints();

    // Provision endpoints
    Log.Information("Provision endpoints mapped");
    Asp.Versioning.Builder.IVersionedEndpointRouteBuilder clouds = app.NewVersionedApi(ProvisionEndpoint.GroupName);
    _ = clouds.MapProvisionV1EndPoint();

    // Bizform endpoints
    Log.Information("Bizform endpoints mapped");
    Asp.Versioning.Builder.IVersionedEndpointRouteBuilder bizforms = app.NewVersionedApi(BizformEndpoint.groupName);
    _ = bizforms.MapBizformV1EndPoint();

    // Uncomment to require authorization
    // provisions.RequireAuthorization(); 
    _ = app.UseDefaultOpenApi();

    Log.Information("Web application started");
    app.Run();
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
