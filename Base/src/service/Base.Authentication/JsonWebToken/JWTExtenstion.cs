using System.Text;
using Base.Domain.Options.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Base.Authentication.JsonWebToken;

public static class JWTExtenstion
{

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.AccessTokenSignKey ?? throw new Exception("設定檔參數錯誤")))
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.RefreshTokenSignKey ?? throw new Exception("設定檔參數錯誤")))
            };
        });
}
