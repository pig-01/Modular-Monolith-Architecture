using System.Globalization;
using System.Security.Claims;
using System.Text;
using Base.Domain.Exceptions;
using Base.Domain.Models.Authentication;
using Base.Domain.Options.Authentication;
using Base.Infrastructure.Interface.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Base.Authentication.JsonWebToken;

/// <summary>
/// Json Web Token 建立服務元件
/// </summary>
public class JwtGenerateService(JWTSetting setting) : IJwtGenerateService
{
    private string Issuer => setting.Issuer ?? throw new ConfigNullException("JWT Issuer 設定異常");
    private string Audience => setting.Audience ?? throw new ConfigNullException("JWT Audience 設定異常");
    private string AccessTokenSignKey => setting.AccessTokenSignKey ?? throw new ConfigNullException("JWT SignKey 設定異常");
    private string RefreshTokenSignKey => setting.RefreshTokenSignKey ?? throw new ConfigNullException("JWT SignKey 設定異常");

    private readonly List<string> roles = [];

    public void AddRole(string role) => roles.Add(role);

    public void AddRoles(IEnumerable<string> roles) => this.roles.AddRange(roles);

    public List<string> GetRoles() => roles;

    public void ClearRoles() => roles.Clear();

    public JwtToken GenerateToken(string userId, string userName, Guid jwtId, int expireMinutes = 30) => GenerateToken(userId, userName, null, jwtId, expireMinutes);

    public JwtToken GenerateToken(string userId, string userName, string? userEmail, Guid jwtId, int expireMinutes = 30)
    {
        long tokenExpressTime = DateTimeOffset.UtcNow.AddMinutes(expireMinutes).ToUnixTimeSeconds();

        // Configuring "Claims" to your JWT Token
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, userEmail ?? ""),

            // In RFC 7519 (Section#4), there are defined 7 built-in Claiss, but we mostly use 2 of them.
            new Claim(JwtRegisteredClaimNames.Iss, Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, Audience),
            new Claim(JwtRegisteredClaimNames.Exp, tokenExpressTime.ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Jti, jwtId.ToString()),
            new Claim(JwtRegisteredClaimNames.Typ, "access")
        ];

        // Add roles to claims
        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        ClaimsIdentity userClaimsIdentity = new(claims);

        // Create a SymmetricSecurityKey for JWT Token signatures
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(AccessTokenSignKey));

        // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
        // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);

        // Create SecurityTokenDescriptor
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = Issuer,
            NotBefore = DateTime.Now,
            IssuedAt = DateTime.Now,
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddMinutes(expireMinutes),
            SigningCredentials = signingCredentials
        };

        // Generate a JWT, than get the serialized Token result (string)
        JsonWebTokenHandler tokenHandler = new();
        string serializeToken = tokenHandler.CreateToken(tokenDescriptor);

        return new JwtToken(serializeToken, tokenExpressTime);
    }

    public JwtToken GenerateRefreshToken(string userId, string userName, Guid jwtId, int expireMinutes = 60) => GenerateRefreshToken(userId, userName, null, jwtId, expireMinutes);

    public JwtToken GenerateRefreshToken(string userId, string userName, string? userEmail, Guid jwtId, int expireMinutes = 60)
    {
        long tokenExpressTime = DateTimeOffset.UtcNow.AddMinutes(expireMinutes).ToUnixTimeSeconds();

        // Configuring "Claims" to your JWT Token
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, userEmail ?? ""),

            // In RFC 7519 (Section#4), there are defined 7 built-in Claiss, but we mostly use 2 of them.
            new Claim(JwtRegisteredClaimNames.Iss, Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, Audience),
            new Claim(JwtRegisteredClaimNames.Exp, tokenExpressTime.ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Jti, jwtId.ToString()),
            new Claim(JwtRegisteredClaimNames.Typ, "refresh")
        ];

        // Add roles to claims
        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        ClaimsIdentity userClaimsIdentity = new(claims);

        // Create a SymmetricSecurityKey for JWT Token signatures
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(RefreshTokenSignKey));

        // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
        // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);

        // Create SecurityTokenDescriptor
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = Issuer,
            NotBefore = DateTime.Now,
            IssuedAt = DateTime.Now,
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddMinutes(expireMinutes),
            SigningCredentials = signingCredentials
        };

        // Generate a JWT, than get the serialized Token result (string)
        JsonWebTokenHandler tokenHandler = new();
        string serializeToken = tokenHandler.CreateToken(tokenDescriptor);

        return new JwtToken(serializeToken, tokenExpressTime);
    }

    public void Dispose()
    {
        roles.Clear();
        GC.SuppressFinalize(this);
    }

    public async Task<ClaimsPrincipal?> GetPrincipalAsync(string accessToken)
    {
        JsonWebTokenHandler tokenHandler = new();
        if (!tokenHandler.CanReadToken(accessToken)) return null;

        // 確認 Token 是否有效
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AccessTokenSignKey))
        };

        TokenValidationResult result = await tokenHandler.ValidateTokenAsync(accessToken, tokenValidationParameters);
        if (result.IsValid)
        {
            return result.ClaimsIdentity != null
                ? new ClaimsPrincipal(result.ClaimsIdentity)
                : null;
        }

        return null;
    }
}
