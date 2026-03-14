namespace AqlliAgronom.Infrastructure.Identity;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "AqlliAgronom";
    public string Audience { get; set; } = "AqlliAgronom.Clients";
    public int AccessTokenExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 30;
}
