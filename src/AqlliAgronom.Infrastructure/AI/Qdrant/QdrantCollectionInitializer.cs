using AqlliAgronom.Application.AI.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AqlliAgronom.Infrastructure.AI.Qdrant;

/// <summary>
/// Ensures Qdrant collection exists on application startup.
/// </summary>
public class QdrantCollectionInitializer(
    IServiceProvider serviceProvider,
    IOptions<QdrantOptions> options,
    ILogger<QdrantCollectionInitializer> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorSearchService>();
        var opts = options.Value;

        try
        {
            await vectorService.EnsureCollectionExistsAsync(
                opts.KnowledgeCollectionName,
                opts.VectorDimension,
                ct);
            logger.LogInformation("Qdrant collection ready: {Collection}", opts.KnowledgeCollectionName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Qdrant collection. Vector search may not work.");
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
