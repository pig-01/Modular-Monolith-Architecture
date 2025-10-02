using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class ClaimExtension
{
    #region Cookie登入用
    /// <summary>
    /// 【Cookie登入】取得登入者名稱
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetSubject(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

    /// <summary>
    /// 【Cookie登入】取得登入者ID
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetNameId(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// 【Cookie登入】取得登入者Email
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetEmail(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
    #endregion

    #region JsonWebToken登入用
    /// <summary>
    /// 【JsonWebToken登入】取得發行網站(人)(單位)
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetIssuer(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss)?.Value;

    /// <summary>
    /// 【JsonWebToken登入】取得使用網站(人)(單位)
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetAudience(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value;

    /// <summary>
    /// 【JsonWebToken登入】取得到期日
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetExpiration(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;

    /// <summary>
    /// 【JsonWebToken登入】取得生效日
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetNotBefore(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Nbf)?.Value;

    /// <summary>
    /// 【JsonWebToken登入】取得簽發時間
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetIssuedAt(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iat)?.Value;

    /// <summary>
    /// 【JsonWebToken登入】取得TokenID
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetJti(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
    #endregion

    #region 客製化
    private const string TimeZone = "time_zone";
    /// <summary>
    /// 使用者時區
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetUserTimeZoneInfo(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == TimeZone)?.Value;

    private const string Culture = "culture";
    /// <summary>
    /// 使用者語系
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static string? GetUserCulture(this IEnumerable<Claim> claims) => claims.FirstOrDefault(x => x.Type == Culture)?.Value;
    #endregion

}
