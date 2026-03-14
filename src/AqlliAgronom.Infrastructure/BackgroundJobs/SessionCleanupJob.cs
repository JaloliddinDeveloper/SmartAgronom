using AqlliAgronom.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Infrastructure.BackgroundJobs;

public class SessionCleanupJob(
    IServiceProvider serviceProvider,
    ILogger<SessionCleanupJob> logger)
    : BackgroundService
{
    private static readonly TimeSpan RunInterval = TimeSpan.FromHours(1);
    private static readonly TimeSpan IdleThreshold = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CloseIdleSessionsAsync(stoppingToken);
            await Task.Delay(RunInterval, stoppingToken);
        }
    }

    private async Task CloseIdleSessionsAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var idleSessions = await uow.FarmerSessions.GetIdleSessionsAsync(IdleThreshold, ct);
        if (idleSessions.Count == 0) return;

        foreach (var session in idleSessions)
            session.Close();

        await uow.SaveChangesAsync(ct);
        logger.LogInformation("Closed {Count} idle sessions", idleSessions.Count);
    }
}
