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
    IFarmerSessionRepository sessionRepository,
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

        // Record user message in session
        session.AddMessage(request.Message, MessageRole.User);

        // Build conversation history for context (last 10 turns)
        var history = session.Messages
            .OrderByDescending(m => m.CreatedAt)
            .Take(10)
            .Reverse()
            .Select(m => new ConversationTurn(
                Role: m.Role.ToString().ToLower(),
                Content: m.Content))
            .ToList();

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

        // Record AI response in session
        session.AddMessage(result.Response, MessageRole.Assistant, result.TotalTokensUsed);

        uow.FarmerSessions.Update(session);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation(
            "AI response generated for Session {SessionId} — {TokensUsed} tokens, {ChunksUsed} knowledge chunks",
            session.Id, result.TotalTokensUsed, result.UsedChunks.Count);

        return new ChatResponseDto(
            SessionId: session.Id,
            Response: result.Response,
            TokensUsed: result.TotalTokensUsed,
            AskingClarification: result.AskingClarification,
            SuggestedProductNames: result.SuggestedProductIds,
            KnowledgeChunksUsed: result.UsedChunks.Count);
    }
}
