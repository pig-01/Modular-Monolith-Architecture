using System.Text;
using Demo.Authentication.CAS;
using Demo.Authentication.CAS.AspNetCore;
using Base.Domain.Exceptions;
using Base.Domain.Models.Authentication;
using Base.Domain.Options.Authentication;
using Base.Infrastructure.Extension;
using Base.Infrastructure.Toolkits.Extensions;
using Main.WebApi.Application.Commands.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using static Base.Domain.Enums.AuthenticationModeEnum;
using Assertion = Demo.Authentication.CAS.Security.Assertion;
using UserDomain = Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Extensions;

/// <summary>
/// 擴展認證
/// </summary>
public static class AuthenticationExtension
{
    /// <summary>
    /// 加入Demo產品基本認證
    /// </summary>
    /// <remarks>
    /// 兩種方式進行認證切換
    /// 1. JWT: 透過 <see cref="v1.Login.LoginController.Login(Dto.ViewModel.Login.LoginRequestDto)"/> 進行認證
    /// 1. CAS: 透過 <see cref="v1.Login.LoginController.CASLogin(string?)"/> 進行連線帳號中心，帳號中心進行認證
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddDemoAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // 取得加密設定
        IConfigurationSection configurationSection = configuration.GetSection(AuthenticationOption.Position);
        services.Configure<AuthenticationOption>(configurationSection);
        AuthenticationOption authenticationOption = configurationSection.Get<AuthenticationOption>()
            .IsNotNull(new ConfigNullException("The configuration value for 'Authentication' is missing or empty. Please provide a valid value in appsettings."))!;

        return authenticationOption.Mode switch
        {
            AuthenticationMode.JWT => services.AddJwtBearerAuthentication(configuration),
            AuthenticationMode.CAS => services.AddCookieAuthentication(configuration, GetDemoUserAsync),
            _ => services.AddAuthentication(),
        };
    }

    public static AuthenticationBuilder AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // 取得加密設定
        IConfigurationSection configurationSection = configuration.GetSection(AuthenticationOption.Position);
        services.Configure<AuthenticationOption>(configurationSection);
        AuthenticationOption authenticationOption = configurationSection.Get<AuthenticationOption>()
            .IsNotNull(new ConfigNullException("The configuration value for 'Authentication' is missing or empty. Please provide a valid value in appsettings."))!;

        return services.AddJwtGenerateService()
            .AddJwtBearer(authenticationOption.JWTSetting);
    }

    public static AuthenticationBuilder AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration, Func<HttpContext, User, Task<User>>? func)
    {
        // 取得加密設定
        IConfigurationSection configurationSection = configuration.GetSection(AuthenticationOption.Position);
        services.Configure<AuthenticationOption>(configurationSection);
        AuthenticationOption authenticationOption = configurationSection.Get<AuthenticationOption>()
            .IsNotNull(new ConfigNullException("The configuration value for 'Authentication' is missing or empty. Please provide a valid value in appsettings."))!;

        authenticationOption.CASSetting.OnCreatingTicketConvertToSCUser = func;
        return services.AddJwtGenerateService().AddCookie(authenticationOption.CASSetting);
    }

    public static AuthenticationBuilder AddCookie(this IServiceCollection services, CASSetting setting) =>
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie()
        .AddCAS("CAS", options =>
        {
            options.CallbackPath = "/signin-cas";
            options.CasServerUrlBase = setting.ServerUrlBase ??
                throw new ConfigNullException("The configuration value for 'ServerUrlBase' is missing or empty. Please provide a valid value in appsettings.");
            options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;
            options.Events.OnAccessDenied = context =>
            {
                context.Response.StatusCode = 401;
                context.HandleResponse();
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAuthorizationEndpoint = context =>
            {
                // 強制轉為 GET 方法
                context.Request.Method = HttpMethods.Get;
                context.Response.Redirect(context.RedirectUri);

                ILogger<CasAuthenticationHandler> logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<CasAuthenticationHandler>>();
                logger.LogDebug("Redirecting to CAS authorization endpoint: {RedirectUri}", context.RedirectUri);

                return Task.CompletedTask;
            };
            options.Events.OnCreatingTicket = async context =>
            {
                ILogger<CasAuthenticationHandler> logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<CasAuthenticationHandler>>();

                logger.LogDebug("Starting CAS ticket creation for user {PrincipalName}", context.Assertion.PrincipalName);

                if (context.Identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    logger.LogDebug("User {PrincipalName} already has NameIdentifier claim, skipping ticket creation", context.Assertion.PrincipalName);
                    return;
                }

                Assertion assertion = context.Assertion;
                User customerDto = new() { UserId = assertion.PrincipalName };

                logger.LogTrace("Extracting CAS attributes for user {UserId}", assertion.PrincipalName);

                if (assertion.Attributes.TryGetValue("display_name", out Microsoft.Extensions.Primitives.StringValues displayName) && !string.IsNullOrWhiteSpace(displayName))
                {
                    customerDto.UserName = displayName;
                    logger.LogTrace("Extracted display_name: {DisplayName} for user {UserId}", displayName, assertion.PrincipalName);
                }

                if (assertion.Attributes.TryGetValue("cn", out Microsoft.Extensions.Primitives.StringValues fullName) && !string.IsNullOrWhiteSpace(fullName))
                {
                    customerDto.UserName = fullName;
                    logger.LogTrace("Extracted cn (common name): {FullName} for user {UserId}", fullName, assertion.PrincipalName);
                }

                if (assertion.Attributes.TryGetValue("email", out Microsoft.Extensions.Primitives.StringValues email) && !string.IsNullOrWhiteSpace(email))
                {
                    customerDto.UserEmail = email;
                    logger.LogTrace("Extracted email: {Email} for user {UserId}", email, assertion.PrincipalName);
                }

                logger.LogDebug("Converting CAS user to 專案user for {UserId}", assertion.PrincipalName);

                User dto = await setting.OnCreatingTicketConvertToSCUser!(context.HttpContext, customerDto);

                logger.LogInformation("Successfully converted CAS user {UserId} ({UserName}) to 專案user", dto.UserId, dto.UserName);

                if (dto.UserTimeZone.IsNullOrEmpty())
                {
                    Log.Warning("HttpContext does not have timezone info. Use default timezone {timezone}", setting.DefaultTimeZoneId);
                }
                if (dto.UserCulture.IsNullOrEmpty())
                {
                    Log.Warning("HttpContext does not have culture info. Use default culture {culture}", setting.DefaultCulture);
                }

                logger.LogTrace("Adding claims for user {UserId}", dto.UserId);

                context.Identity.AddClaim(new Claim("auth_scheme", CasDefaults.AuthenticationType));
                context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, dto.UserId));
                context.Identity.AddClaim(new Claim(ClaimTypes.Name, dto.UserName!));
                context.Identity.AddClaim(new Claim(ClaimTypes.Email, dto.UserEmail!));
                context.Identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                context.Identity.AddClaim(new Claim("time_zone", dto.UserTimeZone ?? setting.DefaultTimeZoneId));
                context.Identity.AddClaim(new Claim("culture", dto.UserCulture ?? setting.DefaultCulture));

                logger.LogInformation("CAS authentication ticket created successfully for user {UserId} ({UserName}) with email {UserEmail}",
                    dto.UserId, dto.UserName, dto.UserEmail);
            };
            options.Events.OnRemoteFailure = context =>
            {
                Exception? failure = context.Failure;
                if (!string.IsNullOrWhiteSpace(failure?.Message))
                {
                    ILogger<CasAuthenticationHandler> logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<CasAuthenticationHandler>>();
                    logger.LogError(failure, "CAS remote failure: {ErrorMessage}", failure.Message);
                }

                context.Response.Redirect("/#/noRight");
                context.HandleResponse();
                return Task.CompletedTask;
            };
        });


    public static AuthenticationBuilder AddJwtBearer(this IServiceCollection services, JWTSetting setting) => services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "AccessToken";
            options.DefaultChallengeScheme = "RefreshToken";
        })
        .AddJwtBearer("AccessToken", options =>
        {
            // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
            options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                // 一般我們都會驗證 Issuer
                ValidateIssuer = true,
                ValidIssuer = setting.ValidIssuer,

                // 通常不太需要驗證 Audience
                ValidateAudience = false,
                //ValidAudience = configurationManager.GetValue<string>("Setting:ValidAudience"), // 不驗證就不需要填寫

                // 一般我們都會驗證 Token 的有效期間
                ValidateLifetime = true,

                // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                ValidateIssuerSigningKey = false,

                // SignKey 應該從 IConfiguration 取得
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.AccessTokenSignKey ??
                    throw new ConfigNullException("The configuration value for 'AccessTokenSignKey' is missing or empty. Please provide a valid value in appsettings.")))
            };

            // 允許從Cookies讀取AccessToken
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("AccessToken"))
                    {
                        context.Token = context.Request.Cookies["AccessToken"];
                    }
                    return Task.CompletedTask;
                }
            };
        }).AddJwtBearer("RefreshToken", options =>
        {
            // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
            options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                // 一般我們都會驗證 Issuer
                ValidateIssuer = true,
                ValidIssuer = setting.ValidIssuer,

                // 驗證 Audience
                ValidateAudience = false,
                ValidAudience = setting.ValidAudience,

                // 一般我們都會驗證 Token 的有效期間
                ValidateLifetime = true,

                // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                ValidateIssuerSigningKey = false,

                // SignKey 應該從 IConfiguration 取得
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.RefreshTokenSignKey ??
                    throw new ConfigNullException("The configuration value for 'RefreshTokenSignKey' is missing or empty. Please provide a valid value in appsettings.")))
            };

            // 允許從Cookies讀取AccessToken
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("RefreshToken"))
                    {
                        context.Token = context.Request.Cookies["RefreshToken"];
                    }
                    return Task.CompletedTask;
                }
            };
        });

    /// <summary>
    /// 提供帳號中心接入帳號中心 <see cref="User"/> 並
    /// 轉換帳號中心帳號資訊為Demo產品帳號 <see cref="User"/>
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="customerDto"></param>
    /// <returns></returns>
    private static async Task<User> GetDemoUserAsync(HttpContext httpContext, User customerDto)
    {
        // 確認 CustomerId 是否存在 Customer 資料表
        IMediator mediator = httpContext.RequestServices.GetRequiredService<IMediator>();

        UserDomain.Scuser scuser = await mediator.Send(new CreateUserCommand
        {
            UserId = customerDto.UserId,
            UserName = customerDto.UserName,
            UserEmail = customerDto.UserEmail,
            UserTimeZone = customerDto.UserTimeZone,
            UserCulture = customerDto.UserCulture
        });

        User User = new()
        {
            UserId = scuser.UserId,
            UserName = scuser.UserName,
            UserEmail = scuser.UserEmail,
            UserTimeZone = httpContext.Request.Headers["time_zone"].FirstOrDefault(),
            UserCulture = httpContext.Request.Headers["culture"].FirstOrDefault()
        };

        return User;
    }
}
