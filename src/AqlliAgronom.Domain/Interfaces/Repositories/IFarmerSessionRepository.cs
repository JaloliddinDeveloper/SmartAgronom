using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Domain.Interfaces.Repositories;

public interface IFarmerSessionRepository : IRepository<FarmerSession>
{
    Task<FarmerSession?> GetActiveSessionByTelegramAsync(long chatId, CancellationToken ct = default);
    Task<FarmerSession?> GetWithMessagesAsync(Guid sessionId, CancellationToken ct = default);
    Task<IReadOnlyList<FarmerSession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<FarmerSession>> GetIdleSessionsAsync(TimeSpan idleThreshold, CancellationToken ct = default);
}
