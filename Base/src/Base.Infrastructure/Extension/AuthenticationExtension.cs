using Base.Authentication.JsonWebToken;
using Base.Domain.Options.Authentication;
using Base.Infrastructure.Interface.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Base.Infrastructure.Extension;

public static class AuthenticationExtension
{
    //public static AuthenticationBuilder AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    //{
    //    // 取得加密設定
    //    IConfigurationSection configurationSection = configuration.GetSection(AuthenticationOption.Position);
    //    services.Configure<AuthenticationOption>(configurationSection);
    //    AuthenticationOption authenticationOption = configurationSection.Get<AuthenticationOption>().IsNotNull(new ConfigNullException("認證設定異常"))!;

    //    return services.AddJwtGenerateService()
    //        .AddJwtBearer(authenticationOption.JWTSetting);
    //}

    public static IServiceCollection AddJwtGenerateService(this IServiceCollection services) => services.AddSingleton<IJwtGenerateService>(serviceProvider =>
    {
        IOptions<AuthenticationOption> authenticationOption = serviceProvider.GetRequiredService<IOptions<AuthenticationOption>>();
        return new JwtGenerateService(authenticationOption.Value.JWTSetting);
    });

    //public static AuthenticationBuilder AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration, Func<HttpContext, User, Task<User>>? func)
    //{
    //    // 取得加密設定
    //    IConfigurationSection configurationSection = configuration.GetSection(AuthenticationOption.Position);
    //    services.Configure<AuthenticationOption>(configurationSection);
    //    AuthenticationOption authenticationOption = configurationSection.Get<AuthenticationOption>().IsNotNull(new ConfigNullException("認證設定異常"))!;

    //    authenticationOption.CASSetting.OnCreatingTicketConvertToSCUser = func;
    //    return services.AddJwtGenerateService().AddCookie(authenticationOption.CASSetting);
    //}
}
