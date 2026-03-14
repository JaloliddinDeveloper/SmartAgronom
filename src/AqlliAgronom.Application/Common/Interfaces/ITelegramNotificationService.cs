namespace AqlliAgronom.Application.Common.Interfaces;

public interface ITelegramNotificationService
{
    Task SendMessageAsync(long chatId, string text, CancellationToken ct = default);
    Task SendMessageWithKeyboardAsync(long chatId, string text, IReadOnlyList<TelegramInlineButton[]> keyboard, CancellationToken ct = default);
    Task SendPhotoAsync(long chatId, string imageUrl, string? caption = null, CancellationToken ct = default);
}

public record TelegramInlineButton(string Text, string CallbackData);
