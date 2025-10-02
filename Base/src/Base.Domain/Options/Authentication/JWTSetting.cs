namespace Base.Domain.Options.Authentication;

public class JWTSetting()
{
    public string Scheme { get; set; } = "Bearer";
    public string ValidIssuer { get; set; } = "Demo.WebAPI";
    public string ValidAudience { get; set; } = "Demo.Web";
    public string Issuer { get; set; } = "Demo.WebAPI";
    public string Audience { get; set; } = "Demo.Web";
    public string AccessTokenSignKey { get; set; } = "**********************************************";
    public string RefreshTokenSignKey { get; set; } = "**********************************************";
    public int ExpireMinutes { get; set; } = 30;
    public int RefreshExpireMinutes { get; set; } = 60;
    public string DefaultTimeZoneId { get; set; } = "UTC";
    public string DefaultCulture { get; set; } = "zh-CHT";
    public bool TwoFactorAuth { get; set; }
}
