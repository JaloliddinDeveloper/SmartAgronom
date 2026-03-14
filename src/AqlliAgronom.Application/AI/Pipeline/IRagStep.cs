namespace AqlliAgronom.Application.AI.Pipeline;

public interface IRagStep
{
    int Order { get; }
    Task ExecuteAsync(RagPipelineContext context, CancellationToken ct);
}
