using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Events.Sessions;

namespace AqlliAgronom.Domain.Entities;

public class FarmerSession : AggregateRoot
{
    public Guid UserId { get; private set; }
    public long TelegramChatId { get; private set; }
    public SessionStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public Guid? ActiveCropId { get; private set; }
    public string? CurrentTopic { get; private set; }
    public int MessageCount { get; private set; }

    private readonly List<ConversationMessage> _messages = new();
    public IReadOnlyCollection<ConversationMessage> Messages => _messages.AsReadOnly();

    private FarmerSession() { }

    public static FarmerSession Start(Guid userId, long telegramChatId)
    {
        var session = new FarmerSession
        {
            UserId = userId,
            TelegramChatId = telegramChatId,
            Status = SessionStatus.Active,
            StartedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(new FarmerSessionStartedEvent(session.Id, userId, telegramChatId));
        return session;
    }

    public ConversationMessage AddMessage(string content, MessageRole role, int tokensUsed = 0, string? modelVersion = null)
    {
        if (Status == SessionStatus.Closed)
            throw new DomainException("SESSION_CLOSED", "Cannot add messages to a closed session.");

        var message = new ConversationMessage(Id, content, role, tokensUsed, modelVersion);
        _messages.Add(message);
        MessageCount++;
        Status = SessionStatus.Active;
        SetUpdatedAt();

        return message;
    }

    public void AttachCrop(Guid cropId)
    {
        ActiveCropId = cropId;
        SetUpdatedAt();
    }

    public void SetTopic(string topic)
    {
        CurrentTopic = topic;
        SetUpdatedAt();
    }

    public void Close()
    {
        if (Status == SessionStatus.Closed) return;
        Status = SessionStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        SetUpdatedAt();
        AddDomainEvent(new FarmerSessionClosedEvent(Id, UserId));
    }

    public void MarkIdle()
    {
        if (Status == SessionStatus.Active)
            Status = SessionStatus.Idle;
    }
}
