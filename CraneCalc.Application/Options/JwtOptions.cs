namespace CraneCalc.Application.Options;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;

    public int ExpiresHours { get; set; }
    
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}