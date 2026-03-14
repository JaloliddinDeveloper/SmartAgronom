using AqlliAgronom.Domain.Entities;
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

    public async Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default)
        => await DbSet.AnyAsync(u => u.Phone.Value == phone, ct);
}
