using System.Reflection;

namespace Main.WebApi.Extensions;

/// <summary>
/// 擴展 AutoMapper
/// </summary>
public static class MapperExtension
{
    /// <summary>
    /// 擴展 AutoMapper
    /// </summary>
    /// <param name="services"></param>
    public static void AddAutoMapper(this IServiceCollection services) =>
        // 註冊 AutoMapper
        services.AddAutoMapper(Assembly.Load("Main.Repository"));
}
