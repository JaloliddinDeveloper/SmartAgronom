using AqlliAgronom.Domain.Interfaces.Repositories;

namespace AqlliAgronom.Application.AI.Pipeline.Steps;

/// <summary>
/// Loads all available products from the database and injects them into the pipeline context
/// so that PromptAssemblyStep can tell Claude exactly which products it may recommend.
/// </summary>
public class ProductInjectionStep(IProductRepository productRepository) : IRagStep
{
    public int Order => 4;

    public async Task ExecuteAsync(RagPipelineContext context, CancellationToken ct)
    {
        var products = await productRepository.GetAvailableAsync(ct);
        context.AvailableProducts = products.Select(p => p.Name).ToList();
    }
}
