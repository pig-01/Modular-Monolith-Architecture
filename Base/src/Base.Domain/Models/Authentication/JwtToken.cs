namespace Base.Domain.Models.Authentication;

public class JwtToken(string token, long tokenExpressTime)
{
    public string Token { get; set; } = token;

    public long TokenExpressTime { get; set; } = tokenExpressTime;
}

