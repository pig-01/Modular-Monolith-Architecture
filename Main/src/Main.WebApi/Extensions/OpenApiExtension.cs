using Main.WebApi.Filters;
using Main.WebApi.Registers;

namespace Main.WebApi.Extensions;

/// <summary>
/// 擴展 OpenApi
/// </summary>
public static class OpenApiExtension
{
    /// <summary>
    /// 擴展 Swagger
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwaggerGen(this IServiceCollection services)
    {
        // 加入 Swagger
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => $"{type.FullName}");
            options.OperationFilter<NotImplementedOperationFilter>();
        });
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();
        services.ConfigureOptions<ConfigureSwaggerUIOptions>();
    }
}
