using System.Data;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Dapper;
using Aspire.ServiceDefaults;
using Base.Infrastructure.Extension;
using Base.Infrastructure.Interface.TimeZone;
using DataHub.Bizform.Extensions;
using DataHub.Cloud.Extensions;
using DataHub.Infrastructure.Application.Behaviors;
using DataHub.Infrastructure.Contexts;
using DataHub.Infrastructure.Handlers;
using DataHub.Infrastructure.Registeries;
using DataHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DataHub.Extensions;

public static partial class Extensions
{
    private const int MajorVersion = 1;
    private const int MinorVersion = 0;

    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.CreateLogger();
        Log.Information("Starting web application");

        builder.Host.UseSerilog();

        builder.AddServiceDefaults();

        // Add the authentication services to DI
        // using DataHub.ServiceDefaults;
        // builder.AddDefaultAuthentication();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        IApiVersioningBuilder apiExplorer = builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(MajorVersion, MinorVersion);
            options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();
            builder.Services.ConfigureOptions<ConfigureSwaggerUIOptions>();
            builder.Services.AddSwaggerGen();
        }

        if (!builder.Environment.IsProduction())
        {
            builder.Services.AddProblemDetails();
        }

        // Configure mediatR
        builder.Services.AddMediatR(cfg =>
        {
            // Register all the assemblies that contain MediatR handlers
            Assembly entryAssembly = Assembly.GetExecutingAssembly();

            // Load all referenced assemblies that contain MediatR handlers
            // This is a simple way to load all assemblies, but you may want to filter them based on your needs
            Assembly[] referencedAssemblies = entryAssembly.GetReferencedAssemblies()
                .Select(Assembly.Load)
                .ToArray();

            cfg.RegisterServicesFromAssemblies(
                [entryAssembly, .. referencedAssemblies]
            );

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        // 加入 EF Core DbContext
        builder.Services.AddDbContext<DbContext, DemoContext>(options =>
        {
            // 使用 SQL Server
            options.UseSqlServer(builder.Configuration.GetConnectionString("BdbuDemoDev2022Connection")!.DecryptString(),
                    providerOptions => { });

            // 設定 EF Core 日誌
            options.EnableSensitiveDataLogging(false)
                .EnableDetailedErrors(false);
        });

        builder.Services.AddDBContextForBase(builder.Configuration);
        builder.Services.AddSqlMapper();
        builder.Services.AddCloudService();
        builder.Services.AddBizformService(builder.Configuration);
        builder.Services.AddMailService();
        builder.Services.DataHubConfigure(builder.Configuration);
    }

    public static void CreateLogger(this WebApplicationBuilder builder)
        => Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateBootstrapLogger();

    public static WebApplication UseDefaultOpenApi(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (ApiVersionDescription description in app.DescribeApiVersions())
                {
                    string url = $"{description.GroupName}/swagger.json";
                    string title = $"Datahub Version {description.ApiVersion.MajorVersion ?? 0}.{description.ApiVersion.MinorVersion ?? 0}";

                    options.SwaggerEndpoint(url, title);
                }
            });
            app.MapSwagger().RequireAuthorization();
        }

        return app;
    }

    public static void AddSqlMapper(this IServiceCollection services)
    {
        services.AddSingleton<ITimeZoneService, TimeZoneService>();

        // Add SqlMapper
        services.AddSingleton<DateTimeHandler>();
        services.AddSingleton<DateTimeOffsetHandler>();
        services.AddSingleton<NullableDateTimeHandler>();
    }

    public static void AddDapperTypeHandler(this WebApplication app)
    {
        SqlMapper.RemoveTypeMap(typeof(DateTime));
        SqlMapper.RemoveTypeMap(typeof(DateTime?));
        SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));

        // Add SqlMapper
        SqlMapper.AddTypeHandler(app.Services.GetRequiredService<DateTimeHandler>());
        SqlMapper.AddTypeHandler(app.Services.GetRequiredService<DateTimeOffsetHandler>());
        SqlMapper.AddTypeHandler(app.Services.GetRequiredService<NullableDateTimeHandler>());
    }
}
