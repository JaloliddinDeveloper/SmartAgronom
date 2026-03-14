using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AqlliAgronom.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AqlliAgronom.Infrastructure.Identity;

public class JwtTokenService(
    IOptions<JwtOptions> options,
    IRefreshTokenStore refreshTokenStore)
    : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GenerateTokenPairAsync(
        User user, CancellationToken ct = default)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiryMinutes);
        var accessToken = GenerateAccessToken(user, expiresAt);
        var refreshToken = GenerateRefreshToken();

        await refreshTokenStore.StoreAsync(
            userId: user.Id,
            tokenHash: HashToken(refreshToken),
            expiresAt: DateTime.UtcNow.AddDays(_options.RefreshTokenExpiryDays),
            ct: ct);

        return (accessToken, refreshToken, expiresAt);
    }

    public async Task<(bool IsValid, Guid? UserId)> ValidateRefreshTokenAsync(
        string token, CancellationToken ct = default)
    {
        var hash = HashToken(token);
        var userId = await refreshTokenStore.ValidateAsync(hash, ct);
        return (userId.HasValue, userId);
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default)
    {
        var hash = HashToken(token);
        await refreshTokenStore.RevokeAsync(hash, ct);
    }

    private string GenerateAccessToken(User user, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("phone", user.Phone.Value),
            new Claim("language", ((int)user.PreferredLanguage).ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
