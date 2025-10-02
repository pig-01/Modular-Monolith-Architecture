using System.Reflection;
using Aspire.ServiceDefaults;
using Base.Infrastructure.Extension;
using Base.Infrastructure.Interface.TimeZone;
using Scheduler.Endpoints;
using Scheduler.Infrastructure;
using Scheduler.Infrastructure.Application.Behaviors;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Scheduler.Extensions;

public static class Extension
{

    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.CreateLogger();
        Log.Information("Web application starting up");

        _ = builder.Services.AddOpenApi("scheduler", options =>
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                IConfigurationSection openApi = builder.Configuration.GetSection("OpenApi");

                if (!openApi.Exists()) return Task.CompletedTask;

                document.Info = GetOpenApiInfo(openApi);
                return Task.CompletedTask;
            }));

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMailService();
        builder.Host.UseSerilog();
        builder.AddServiceDefaults();
        builder.ConfigureServices();
        builder.AddQuartzScheduler();
        builder.AddApplicationServiceWithAutofac();

        // Configure mediatR
        builder.Services.AddMediatR(cfg =>
        {
            // Register all the assemblies that contain MediatR handlers
            Assembly entryAssembly = Assembly.GetExecutingAssembly();

            // Load all referenced assemblies that contain MediatR handlers
            // This is a simple way to load all assemblies, but you may want to filter them based on your needs
            Assembly[] referencedAssemblies = [.. entryAssembly.GetReferencedAssemblies().Select(Assembly.Load)];

            _ = cfg.RegisterServicesFromAssemblies(
                [entryAssembly, .. referencedAssemblies]
            );

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        builder.AddDbContext();

        _ = builder.Services.AddScoped<ITimeZoneService, TimeZoneService>();

        return builder;
    }

    /// <summary>
    /// Gets the OpenApi information from the configuration section.
    /// </summary>
    /// <param name="openApi"></param>
    /// <returns></returns>
    private static OpenApiInfo GetOpenApiInfo(IConfigurationSection openApi)
    {
        string defaultString = "Default String";

        OpenApiInfo openApiInfo = new()
        {
            Title = openApi.GetValue("Document:Title", defaultString),
            Description = openApi.GetValue("Document:Description", defaultString),
            Contact = new OpenApiContact
            {
                Name = openApi.GetValue("Document:Contact:Name", defaultString),
                Email = openApi.GetValue("Document:Contact:Email", defaultString),
                Url = new Uri(openApi.GetValue("Document:Contact:Url", defaultString) ?? defaultString)
            },
            License = new OpenApiLicense
            {
                Name = openApi.GetValue("Document:License:Name", defaultString),
                Url = new Uri(openApi.GetValue("Document:License:Url", defaultString) ?? defaultString)
            },
            Version = openApi.GetValue("Document:Version", defaultString)
        };

        return openApiInfo;
    }

    public static void CreateLogger(this WebApplicationBuilder builder) => Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {

        _ = app.UseHttpsRedirection();

        return app;
    }

    public static WebApplication UseDefaultOpenApi(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            _ = app.MapOpenApi("/openapi/{documentName}/openapi.json");

            _ = app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/openapi/scheduler/openapi.json", "Scheduler API V1");
                c.RoutePrefix = "swagger"; // Set Swagger UI at the app's root

                // 顯示請求時間
                c.DisplayRequestDuration();

                // 啟動深度連結
                c.EnableDeepLinking();

                // 啟動篩選框
                c.EnableFilter();

                // 預設模型展開深度，0為不展開，預設為1
                c.DefaultModelsExpandDepth(-3);

                // 文件展開設定
                c.DocExpansion(DocExpansion.List);
            });
        }

        return app;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Map default endpoints
        _ = app.MapGet("/", () => "Welcome to Scheduler API")
            .WithName("Home")
            .WithSummary("Home endpoint")
            .WithDescription("This is the home endpoint of the Scheduler API.")
            .ExcludeFromDescription();

        return app;
    }

    public static WebApplication MapSchedulerEndpoints(this WebApplication app)
    {
        // Map Quartz scheduler endpoints
        Log.Information("Scheduler endpoints mapped");
        app.MapSchedulerV1Endpoint();

        return app;
    }
}