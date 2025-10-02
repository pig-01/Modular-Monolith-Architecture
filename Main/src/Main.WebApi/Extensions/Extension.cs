using System.Net;
using System.Text.Json.Serialization;
using Asp.Versioning.ApiExplorer;
using Aspire.ServiceDefaults;
using Base.Domain.Exceptions;
using Base.Domain.Options.FrontEnd;
using Base.Infrastructure.Extension;
using Base.Infrastructure.Toolkits.Converter.Json;
using Base.Infrastructure.Toolkits.Extensions;
using Main.Infrastructure.Options.Api;
using Main.Infrastructure.Options.Bizform;
using Main.Infrastructure.Options.NetZero;
using Main.Infrastructure.Options.System;
using Main.WebApi.Application.Hubs;
using Main.WebApi.Attributes;
using Main.WebApi.Conventions;
using Microsoft.AspNetCore.Diagnostics;

namespace Main.WebApi.Extensions;

public static class Extension
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.CreateLogger();
        Log.Information("Starting web application");

        builder.AddServiceWithEnvironment();
        builder.AddServiceDefaults();
        builder.Host.UseSerilog();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAutoMapper();
        builder.Services.AddSqlMapper();
        builder.Services.AddMailService();
        builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
        builder.Services.AddSignalRServices();

        // Add Data Provider
        builder.AddDbContext();

        // Add Demo Authentication and Authorization
        builder.AddDemoAuthentication();

        // Add Demo Base Service
        builder.AddBaseService();
        builder.AddSystemOptions();
        builder.AddSystemHttpClient();

        // Add ApiVersion
        builder.AddCors();
        builder.AddApiVersion();
        builder.AddMediatR();

        builder.AddApplicationServiceWithAutofac();
        builder.AddApplicationController();

        //Add Aspose License
        AsposeExtension.LoadLicense(builder.Configuration);

        return builder;
    }

    public static void CreateLogger(this WebApplicationBuilder builder) => Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

    public static void AddServiceWithEnvironment(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwaggerGen();
            builder.Services.AddProblemDetails();
        }
        else
        {
            builder.Services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = Status308PermanentRedirect;
                options.HttpsPort = 443;
            });
        }
    }

    public static void AddDemoAuthentication(this WebApplicationBuilder builder)
    {
        // Add Demo Authentication
        builder.Services.AddDemoAuthentication(builder.Configuration);

        // Add Authorization
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
            .AddPolicy("User", policy => policy.RequireRole("User"));
    }

    public static void AddApplicationController(this WebApplicationBuilder builder)
    {
        // Add Controllers
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new CustomControllerNameConvention());
            options.Filters.Add<CustomExceptionFilterAttribute>();
        }).AddJsonOptions(options =>
        {
            foreach (JsonConverter converter in ConverterBase.Settings.Converters)
            {
                options.JsonSerializerOptions.Converters.Add(converter);
            }
            options.JsonSerializerOptions.IncludeFields = true;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        // Add Endpoint
        builder.Services.AddEndpointsApiExplorer();
    }

    public static void AddSystemOptions(this WebApplicationBuilder builder)
    {
        // Add System Options for Bizform
        IConfigurationSection bizformConfigurationSection = builder.Configuration.GetSection(BizformOption.Position);
        builder.Services.Configure<BizformOption>(bizformConfigurationSection);

        // Add System Options for NetZero
        IConfigurationSection netzeroConfigurationSection = builder.Configuration.GetSection(NetZeroOption.Position);
        builder.Services.Configure<NetZeroOption>(netzeroConfigurationSection);

        // Add System Options for API CORS
        IConfigurationSection corsConfigurationSection = builder.Configuration.GetSection(CorsOption.Position);
        builder.Services.Configure<CorsOption>(corsConfigurationSection);

        // Add Frontend Options
        IConfigurationSection frontendConfigurationSection = builder.Configuration.GetSection(FrontEndOption.Position);
        builder.Services.Configure<FrontEndOption>(frontendConfigurationSection);

        // Add System Options
        IConfigurationSection systemConfigurationSection = builder.Configuration.GetSection(SystemOption.Position);
        builder.Services.Configure<SystemOption>(systemConfigurationSection);
    }

    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (ApiVersionDescription description in app.DescribeApiVersions())
                {
                    string url = $"{description.GroupName}/swagger.json";
                    string title = $"WebAPI v{description.ApiVersion.MajorVersion ?? 0}.{description.ApiVersion.MinorVersion ?? 0}";

                    options.SwaggerEndpoint(url, title);
                }
            });
        }
        else
        {
            _ = app.UseExceptionHandler(appBuilder => appBuilder.Run(async context =>
                {
                    // Handle exceptions and return a JSON response
                    IExceptionHandlerPathFeature? exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    Exception exception = exceptionHandlerPathFeature is not null ? exceptionHandlerPathFeature.Error : new HandleException("An unexpected error occurred.");

                    var response = exception switch
                    {
                        OperationCanceledException => new
                        {
                            status = (int)HttpStatusCode.BadRequest,
                            message = "The operation was canceled.",
                            detail = exception.Message
                        },
                        _ => new
                        {
                            status = (int)HttpStatusCode.InternalServerError,
                            message = "An unexpected error occurred.",
                            detail = exception.Message
                        }
                    };

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = response.status;
                    await context.Response.WriteAsync(response.ToJson());
                }));
        }

        app.UseStatusCodePages();

        // Enable CORS with the specified policy
        app.UseDemoCrossOrigin();
        app.AddDapperTypeHandler();

        app.UseSerilogRequestLogging("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRootApi();
        app.MapControllers();
        app.MapSignalRHubs();

        return app;
    }

    private static void MapRootApi(this WebApplication app) =>
        // Map the root API
        app.MapGet("/", (HttpContext context) =>
        {
            // Redirect to Swagger UI when environment is development
            if (app.Environment.IsDevelopment())
            {
                return Results.Redirect("/swagger");
            }
            else
            {
                // Show a Authorization infomation when not in development
                ClaimsPrincipal user = context.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    var response = new
                    {
                        Message = "Welcome to the API!",
                        User = user.Identity.Name
                    };

                    return Results.Json(response);
                }
                else
                {
                    return Results.Json(new
                    {
                        Message = "Unauthorized access. Please log in to view this information."
                    }, statusCode: Status401Unauthorized);
                }
            }
        });
}
