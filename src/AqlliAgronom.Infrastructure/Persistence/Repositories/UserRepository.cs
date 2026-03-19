using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext dbContext)
    : GenericRepository<User>(dbContext), IUserRepository
{
    public async Task<User?> FindByPhoneAsync(string phone, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(u => u.Phone.Value == phone, ct);

    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<User?> FindByTelegramChatIdAsync(long chatId, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(u => u.TelegramChatId == chatId, ct);

    public async Task<User?> FindByTelegramUsernameAsync(string username, CancellationToken ct = default)
    {
        var normalized = username.TrimStart('@').ToLowerInvariant();
        return await DbSet.FirstOrDefaultAsync(
            u => u.TelegramUsername != null &&
                 u.TelegramUsername.ToLower() == normalized, ct);
    }

    public async Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default)
        => await DbSet.AnyAsync(u => u.Phone.Value == phone, ct);

    public async Task<IReadOnlyList<User>> GetAgronomistsWithTelegramAsync(CancellationToken ct = default)
        => await DbSet
            .Where(u => (u.Role == UserRole.Agronom || u.Role == UserRole.Admin)
                        && u.TelegramChatId != null && u.IsActive)
            .ToListAsync(ct);
}
