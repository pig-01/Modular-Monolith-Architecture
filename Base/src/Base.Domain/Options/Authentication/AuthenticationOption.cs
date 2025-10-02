using static Base.Domain.Enums.AuthenticationModeEnum;

namespace Base.Domain.Options.Authentication;

public class AuthenticationOption()
{
    public const string Position = "Authentication";

    public AuthenticationMode Mode { get; set; } = AuthenticationMode.JWT;

    public JWTSetting JWTSetting { get; set; } = new JWTSetting();
    public CASSetting CASSetting { get; set; } = new CASSetting();
}
