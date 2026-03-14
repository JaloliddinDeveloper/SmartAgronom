namespace AqlliAgronom.Infrastructure.Identity;

public interface IRefreshTokenStore
{
    Task StoreAsync(Guid userId, string tokenHash, DateTime expiresAt, CancellationToken ct = default);
    Task<Guid?> ValidateAsync(string tokenHash, CancellationToken ct = default);
    Task RevokeAsync(string tokenHash, CancellationToken ct = default);
}
