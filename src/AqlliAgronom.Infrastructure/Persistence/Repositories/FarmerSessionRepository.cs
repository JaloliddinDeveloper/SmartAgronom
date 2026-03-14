using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence.Repositories;

public class FarmerSessionRepository(ApplicationDbContext dbContext)
    : GenericRepository<FarmerSession>(dbContext), IFarmerSessionRepository
{
    public async Task<FarmerSession?> GetActiveSessionByTelegramAsync(long chatId, CancellationToken ct = default)
        => await DbSet
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s =>
                s.TelegramChatId == chatId &&
                (s.Status == SessionStatus.Active || s.Status == SessionStatus.Idle), ct);

    public async Task<FarmerSession?> GetWithMessagesAsync(Guid sessionId, CancellationToken ct = default)
        => await DbSet
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

    public async Task<IReadOnlyList<FarmerSession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await DbSet
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<FarmerSession>> GetIdleSessionsAsync(TimeSpan idleThreshold, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow - idleThreshold;
        return await DbSet
            .Where(s => s.Status == SessionStatus.Active &&
                        s.UpdatedAt < cutoff)
            .ToListAsync(ct);
    }
}
