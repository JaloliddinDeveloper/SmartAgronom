namespace AqlliAgronom.Infrastructure.Telegram;

public class TelegramBotOptions
{
    public const string SectionName = "Telegram";

    public string BotToken { get; set; } = string.Empty;
    public string? WebhookUrl { get; set; }
    public string? WebhookSecret { get; set; }
    public long? AdminChatId { get; set; }
    public bool UseWebhook { get; set; } = true;
}
