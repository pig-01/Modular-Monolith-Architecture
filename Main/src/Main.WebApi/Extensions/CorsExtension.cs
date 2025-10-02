using Main.Infrastructure.Options.Api;
using Microsoft.Extensions.Options;

namespace Main.WebApi.Extensions;

/// <summary>
/// 擴展 CORS
/// </summary>
public static class CorsExtension
{
    /// <summary>
    /// 建立 CORS
    /// </summary>
    /// <param name="builder"></param>
    /// <exception cref="Exception"></exception>
    public static void AddCors(this WebApplicationBuilder builder) =>
        // Add CORS services with a named policy
        builder.Services.AddCors(options =>
        {
            CorsOption corsOptions = builder.Configuration.GetSection(CorsOption.Position)
                .Get<CorsOption>() ?? throw new Exception("CorsOption is null");

            corsOptions.PolicySettings.ForEach(corsSetting => options.AddPolicy(corsSetting.PolicyName, policy => policy
                        .WithOrigins(corsSetting.Origins)
                        .WithMethods(corsSetting.Methods)
                        .WithHeaders(corsSetting.Headers)
                        .AllowCredentials()));
        });

    /// <summary>
    /// 使用 CORS
    /// </summary>
    /// <param name="app"></param>
    public static void UseDemoCrossOrigin(this WebApplication app)
    {
        IOptionsMonitor<CorsOption> options = app.Services.GetRequiredService<IOptionsMonitor<CorsOption>>();
        app.UseCors(options.CurrentValue.CorsPolicyName);
    }
}
