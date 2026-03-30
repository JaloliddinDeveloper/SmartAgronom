using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence.Repositories;

public class EduPlayerRepository(ApplicationDbContext db) : IEduPlayerRepository
{
    public Task<int> CountAsync(CancellationToken ct = default) =>
        db.EduPlayers.CountAsync(ct);

    public async Task RegisterIfNewAsync(string name, CancellationToken ct = default)
    {
        var exists = await db.EduPlayers.AnyAsync(p => p.Name == name, ct);
        if (!exists)
        {
            db.EduPlayers.Add(EduPlayer.Register(name));
            await db.SaveChangesAsync(ct);
        }
    }
}
