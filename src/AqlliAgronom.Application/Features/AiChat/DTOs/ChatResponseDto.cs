namespace AqlliAgronom.Application.Features.AiChat.DTOs;

public record ChatResponseDto(
    Guid SessionId,
    string Response,
    int TokensUsed,
    bool AskingClarification,
    IReadOnlyList<string> SuggestedProductNames,
    int KnowledgeChunksUsed);

public record SessionDto(
    Guid Id,
    Guid UserId,
    string Status,
    DateTime StartedAt,
    DateTime? ClosedAt,
    int MessageCount);
