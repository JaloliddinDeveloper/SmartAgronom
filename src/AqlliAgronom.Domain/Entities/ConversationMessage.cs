using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Domain.Entities;

public class ConversationMessage : BaseEntity
{
    public Guid SessionId { get; private set; }
    public MessageRole Role { get; private set; }
    public string Content { get; private set; } = default!;
    public int TokensUsed { get; private set; }
    public string? ModelVersion { get; private set; }

    private ConversationMessage() { }

    internal ConversationMessage(Guid sessionId, string content, MessageRole role, int tokensUsed, string? modelVersion)
    {
        SessionId = sessionId;
        Content = content;
        Role = role;
        TokensUsed = tokensUsed;
        ModelVersion = modelVersion;
    }
}
