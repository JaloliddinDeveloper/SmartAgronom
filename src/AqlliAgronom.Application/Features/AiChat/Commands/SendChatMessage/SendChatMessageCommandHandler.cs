using AqlliAgronom.Application.AI.Interfaces;
using AqlliAgronom.Application.AI.Pipeline;
using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Features.AiChat.DTOs;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.AiChat.Commands.SendChatMessage;

public class SendChatMessageCommandHandler(
    IUnitOfWork uow,
    IRagPipelineService ragPipeline,
    ILogger<SendChatMessageCommandHandler> logger)
    : IRequestHandler<SendChatMessageCommand, ChatResponseDto>
{
    public async Task<ChatResponseDto> Handle(SendChatMessageCommand request, CancellationToken ct)
    {
        // Load session with message history
        var session = await uow.FarmerSessions.GetWithMessagesAsync(request.SessionId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.FarmerSession), request.SessionId);

        if (session.UserId != request.UserId)
            throw new ForbiddenException("You do not have access to this session.");

        // Load user for language preference
        var user = await uow.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);

        // Record user message in session and explicitly track as Added.
        // EF Core 8 discovers entities in PropertyAccessMode.Field navigations during
        // DetectChanges but may assign Unchanged state (not Added) to entities with
        // non-sentinel Guid PKs, causing UPDATE→0 rows→DbUpdateConcurrencyException.
        var userMessage = session.AddMessage(request.Message, MessageRole.User);
        uow.Add(userMessage);

        // Build conversation history for context (last 10 turns)
        var history = session.Messages
            .OrderByDescending(m => m.CreatedAt)
            .Take(10)
            .Reverse()
            .Select(m => new ConversationTurn(
                Role: m.Role.ToString().ToLower(),
                Content: m.Content))
            .ToList();

        // Persist user message before the long-running RAG call.
        // Without this, the 40-60 s RAG window causes EF's DbContext to produce
        // a DbUpdateConcurrencyException on the final SaveChanges because the
        // change-tracker state becomes stale relative to what was actually committed.
        await uow.SaveChangesAsync(ct);

        // Build RAG context
        var pipelineContext = new RagPipelineContext
        {
            UserQuery = request.Message,
            SessionId = session.Id,
            UserId = request.UserId,
            UserLanguage = user.PreferredLanguage,
            RecentHistory = history
        };

        // Execute full RAG pipeline: preprocess → embed → search → rank → prompt → generate → postprocess
        var result = await ragPipeline.ExecuteAsync(pipelineContext, ct);

        // Record AI response in session (same explicit tracking fix)
        var aiMessage = session.AddMessage(result.Response, MessageRole.Assistant, result.TotalTokensUsed);
        uow.Add(aiMessage);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation(
            "AI response generated for Session {SessionId} — {TokensUsed} tokens, {ChunksUsed} knowledge chunks",
            session.Id, result.TotalTokensUsed, result.UsedChunks.Count);

        return new ChatResponseDto(
            SessionId: session.Id,
            Response: result.Response,
            TokensUsed: result.TotalTokensUsed,
            AskingClarification: result.AskingClarification,
            SuggestedProductNames: result.SuggestedProductIds ?? [],
            KnowledgeChunksUsed: result.UsedChunks.Count);
    }
}
