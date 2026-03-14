using AqlliAgronom.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AqlliAgronom.Infrastructure.Telegram;

public class TelegramBotService(
    ITelegramBotClient botClient,
    ILogger<TelegramBotService> logger)
    : ITelegramNotificationService
{
    public async Task SendMessageAsync(long chatId, string text, CancellationToken ct = default)
    {
        try
        {
            if (text.Length <= 4096)
            {
                await botClient.SendMessage(chatId, text,
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
            }
            else
            {
                var chunks = SplitMessage(text, 4000);
                foreach (var chunk in chunks)
                {
                    await botClient.SendMessage(chatId, chunk,
                        parseMode: ParseMode.Markdown,
                        cancellationToken: ct);
                    await Task.Delay(300, ct);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send Telegram message to chat {ChatId}", chatId);
            throw;
        }
    }

    public async Task SendMessageWithKeyboardAsync(
        long chatId, string text,
        IReadOnlyList<TelegramInlineButton[]> keyboard,
        CancellationToken ct = default)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(
            keyboard.Select(row =>
                row.Select(btn => InlineKeyboardButton.WithCallbackData(btn.Text, btn.CallbackData))));

        await botClient.SendMessage(chatId, text,
            replyMarkup: inlineKeyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: ct);
    }

    public async Task SendPhotoAsync(
        long chatId, string imageUrl, string? caption = null, CancellationToken ct = default)
    {
        await botClient.SendPhoto(chatId,
            photo: InputFile.FromUri(imageUrl),
            caption: caption,
            parseMode: ParseMode.Markdown,
            cancellationToken: ct);
    }

    private static IEnumerable<string> SplitMessage(string text, int maxLength)
    {
        for (int i = 0; i < text.Length; i += maxLength)
            yield return text.Substring(i, Math.Min(maxLength, text.Length - i));
    }
}
