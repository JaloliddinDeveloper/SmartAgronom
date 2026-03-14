using System.Text.RegularExpressions;

namespace AqlliAgronom.Application.AI.Pipeline.Steps;

public class ResponsePostprocessingStep : IRagStep
{
    public int Order => 5;

    private static readonly Regex ProductTagRegex =
        new(@"\[\[([^\]]+)\]\]", RegexOptions.Compiled);

    private static readonly string[] ClarificationIndicators =
        ["clarify", "could you", "please provide", "what crop", "which region", "aniqlashtiring", "qaysi", "уточните"];

    public Task ExecuteAsync(RagPipelineContext context, CancellationToken ct)
    {
        var response = context.FinalResponse;

        // Detect if AI is asking a clarifying question rather than giving diagnosis
        context.AskingClarification = ClarificationIndicators
            .Any(indicator => response.Contains(indicator, StringComparison.OrdinalIgnoreCase));

        // Extract product suggestions from [[ProductName]] markers
        var productMatches = ProductTagRegex.Matches(response);
        var productNames = productMatches
            .Select(m => m.Groups[1].Value.Trim())
            .Distinct()
            .ToList();
        context.SuggestedProductIds = productNames;

        // Clean up markers from final response shown to user
        context.FinalResponse = ProductTagRegex.Replace(response, m => $"**{m.Groups[1].Value}**");

        return Task.CompletedTask;
    }
}
