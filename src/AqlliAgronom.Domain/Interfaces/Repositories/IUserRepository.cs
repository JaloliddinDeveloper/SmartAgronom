using AqlliAgronom.Domain.Entities;

namespace AqlliAgronom.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByPhoneAsync(string phone, CancellationToken ct = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> FindByTelegramChatIdAsync(long chatId, CancellationToken ct = default);
    Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAgronomistsWithTelegramAsync(CancellationToken ct = default);
}
