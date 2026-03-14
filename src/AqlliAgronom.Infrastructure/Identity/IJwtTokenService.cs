using AqlliAgronom.Domain.Entities;

namespace AqlliAgronom.Infrastructure.Identity;

public interface IJwtTokenService
{
    Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GenerateTokenPairAsync(
        User user, CancellationToken ct = default);

    Task<(bool IsValid, Guid? UserId)> ValidateRefreshTokenAsync(string token, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default);
}
