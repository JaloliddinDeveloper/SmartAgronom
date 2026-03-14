using AqlliAgronom.Application.AI.Interfaces;
using AqlliAgronom.Application.AI.Pipeline;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Infrastructure.AI;

/// <summary>
/// Orchestrates all IRagStep instances in their defined Order,
/// then calls Claude AI with the assembled prompt and context.
/// </summary>
public class RagPipelineService(
    IEnumerable<IRagStep> steps,
    IClaudeAiService claudeService,
    ILogger<RagPipelineService> logger)
    : IRagPipelineService
{
    private readonly IReadOnlyList<IRagStep> _steps = steps.OrderBy(s => s.Order).ToList();

    public async Task<RagPipelineResult> ExecuteAsync(RagPipelineContext context, CancellationToken ct)
    {
        logger.LogDebug("RAG pipeline started for session {SessionId}. Query: {Query}",
            context.SessionId, context.UserQuery[..Math.Min(100, context.UserQuery.Length)]);

        // Separate postprocessing step (must run after Claude response is populated)
        var postStep = _steps.LastOrDefault(s => s.GetType().Name.Contains("Postprocessing"));

        // Execute all pipeline steps except postprocessing (which needs the AI response first)
        foreach (var step in _steps.Where(s => s != postStep))
        {
            var stepName = step.GetType().Name;
            try
            {
                await step.ExecuteAsync(context, ct);
                logger.LogDebug("RAG step [{Order}] {StepName} completed", step.Order, stepName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RAG step [{Order}] {StepName} failed", step.Order, stepName);
                if (step.Order <= 2) throw; // Critical steps: preprocessing and retrieval
                // Non-critical steps: continue with degraded context
            }
        }

        // Build conversation messages for Claude
        var messages = new List<AiMessage>();

        // Add recent history
        foreach (var turn in context.RecentHistory)
            messages.Add(new AiMessage(turn.Role, turn.Content));

        // Add current user query
        messages.Add(new AiMessage("user", context.UserQuery));

        // Call Claude with assembled system prompt
        var result = await claudeService.CompleteAsync(
            messages: messages,
            systemPrompt: context.AssembledSystemPrompt,
            ct: ct);

        context.FinalResponse = result.Content;
        context.TotalTokensUsed = result.InputTokens + result.OutputTokens;

        // Now run postprocessing with the populated response
        if (postStep is not null)
            await postStep.ExecuteAsync(context, ct);

        logger.LogInformation(
            "RAG pipeline completed. Tokens: {Tokens}, Chunks: {Chunks}, Clarifying: {Clarifying}",
            context.TotalTokensUsed, context.RankedChunks.Count, context.AskingClarification);

        return new RagPipelineResult(
            Response: context.FinalResponse,
            TotalTokensUsed: context.TotalTokensUsed,
            UsedChunks: context.RankedChunks,
            AskingClarification: context.AskingClarification,
            SuggestedProductIds: context.SuggestedProductIds);
    }
}
