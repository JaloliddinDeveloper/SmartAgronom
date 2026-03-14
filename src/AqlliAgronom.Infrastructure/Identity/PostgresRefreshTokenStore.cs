using AqlliAgronom.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Identity;

/// <summary>
/// Stores hashed refresh tokens in PostgreSQL.
/// </summary>
public class PostgresRefreshTokenStore(ApplicationDbContext dbContext) : IRefreshTokenStore
{
    public async Task StoreAsync(Guid userId, string tokenHash, DateTime expiresAt, CancellationToken ct = default)
    {
        var record = new RefreshTokenRecord
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
        dbContext.Set<RefreshTokenRecord>().Add(record);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<Guid?> ValidateAsync(string tokenHash, CancellationToken ct = default)
    {
        var record = await dbContext.Set<RefreshTokenRecord>()
            .FirstOrDefaultAsync(r =>
                r.TokenHash == tokenHash &&
                !r.IsRevoked &&
                r.ExpiresAt > DateTime.UtcNow, ct);

        return record?.UserId;
    }

    public async Task RevokeAsync(string tokenHash, CancellationToken ct = default)
    {
        var record = await dbContext.Set<RefreshTokenRecord>()
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, ct);

        if (record is not null)
        {
            record.IsRevoked = true;
            await dbContext.SaveChangesAsync(ct);
        }
    }
}

public class RefreshTokenRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }
}
