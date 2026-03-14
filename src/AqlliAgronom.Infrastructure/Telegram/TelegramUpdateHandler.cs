using AqlliAgronom.Application.Features.AiChat.Commands.SendChatMessage;
using AqlliAgronom.Application.Features.AiChat.Commands.StartSession;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AqlliAgronom.Infrastructure.Telegram;

public class TelegramUpdateHandler(
    ITelegramBotClient botClient,
    ISender mediator,
    IUserRepository userRepository,
    ILogger<TelegramUpdateHandler> logger)
    : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var task = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(update.Message!, ct),
            UpdateType.CallbackQuery => HandleCallbackQueryAsync(update.CallbackQuery!, ct),
            _ => Task.CompletedTask
        };

        await task;
    }

    private async Task HandleMessageAsync(Message message, CancellationToken ct)
    {
        if (message.Text is null) return;

        var chatId = message.Chat.Id;
        var text = message.Text.Trim();

        logger.LogInformation("Telegram message from chat {ChatId}: {Text}",
            chatId, text[..Math.Min(100, text.Length)]);

        // Find registered user by Telegram chat ID
        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);

        if (text.StartsWith("/start"))
        {
            await HandleStartCommandAsync(chatId, message, user, ct);
            return;
        }

        if (text.StartsWith("/help"))
        {
            await SendHelpMessageAsync(chatId, ct);
            return;
        }

        if (user is null)
        {
            await botClient.SendTextMessageAsync(chatId,
                "Siz ro'yxatdan o'tmagansiz. Iltimos, avval ro'yxatdan o'ting: /start",
                cancellationToken: ct);
            return;
        }

        // Start or resume session
        var session = await mediator.Send(new StartSessionCommand(user.Id, chatId), ct);

        // Send typing indicator
        await botClient.SendChatActionAsync(chatId, ChatAction.Typing, cancellationToken: ct);

        try
        {
            // Process message through AI RAG pipeline
            var response = await mediator.Send(
                new SendChatMessageCommand(session.Id, text, user.Id), ct);

            await botClient.SendTextMessageAsync(chatId, response.Response,
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            // If products recommended, show product buttons
            if (response.SuggestedProductNames.Count > 0)
            {
                await SendProductSuggestionsAsync(chatId, response.SuggestedProductNames, ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing AI response for chat {ChatId}", chatId);
            await botClient.SendTextMessageAsync(chatId,
                "Kechirasiz, xatolik yuz berdi. Iltimos, qayta urinib ko'ring.",
                cancellationToken: ct);
        }
    }

    private async Task HandleStartCommandAsync(
        long chatId, Message message, Domain.Entities.User? user, CancellationToken ct)
    {
        var firstName = message.From?.FirstName ?? "Dehqon";

        if (user is not null)
        {
            await botClient.SendTextMessageAsync(chatId,
                $"Xush kelibsiz, {user.FullName}! 🌱\n\n" +
                "Men AqlliAgronom AI — sizning aqlli agronomiyst yordamchingizman.\n" +
                "Ekinlaringiz bilan bog'liq muammolarni yozing va men yordam beraman.",
                cancellationToken: ct);
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId,
                $"Assalomu alaykum, {firstName}! 🌾\n\n" +
                "Men AqlliAgronom AI — aqlli agronomiyst yordamchisi.\n\n" +
                "Foydalanish uchun avval ro'yxatdan o'ting.\n" +
                "Saytimizga o'ting va ro'yxatdan o'ting, so'ngra Telegram ID'ingizni bog'lang.",
                cancellationToken: ct);
        }
    }

    private async Task SendHelpMessageAsync(long chatId, CancellationToken ct)
    {
        var helpText = """
            🌱 *AqlliAgronom AI — Qo'llanma*

            Men sizga quyidagilarda yordam bera olaman:

            🔍 *Kasallik va zararkunandalarni aniqlash*
            • Ekinlaringiz muammolarini tasvirlab yozing
            • Men tahlil qilib, davolash usullarini tavsiya etaman

            💊 *Mahsulot tavsiyalari*
            • Kimyoviy va biologik preparatlar
            • Qo'llash miqdori va usuli

            📦 *Buyurtma berish*
            • Tavsiya etilgan mahsulotlarni bevosita buyurtma bering

            💬 *Savol berish*
            Shunchaki savolingizni yozing, men javob beraman!
            """;

        await botClient.SendTextMessageAsync(chatId, helpText,
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task SendProductSuggestionsAsync(
        long chatId, IReadOnlyList<string> productNames, CancellationToken ct)
    {
        var text = "📦 *Tavsiya etilgan mahsulotlar:*";
        var buttons = productNames.Take(3).Select(name =>
            new[] { new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton(name)
            {
                CallbackData = $"product:{name}"
            }
        }).ToArray();

        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons);
        await botClient.SendTextMessageAsync(chatId, text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: ct);
    }

    private async Task HandleCallbackQueryAsync(CallbackQuery query, CancellationToken ct)
    {
        await botClient.AnswerCallbackQueryAsync(query.Id, cancellationToken: ct);

        if (query.Data?.StartsWith("product:") == true)
        {
            var productName = query.Data[8..];
            await botClient.SendTextMessageAsync(query.Message!.Chat.Id,
                $"📦 *{productName}* haqida ma'lumot olish uchun iltimos, veb-saytimizga tashrif buyuring yoki quyida buyurtma bering.",
                cancellationToken: ct);
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiEx => $"Telegram API Error [{apiEx.ErrorCode}]: {apiEx.Message}",
            _ => exception.ToString()
        };

        logger.LogError("Telegram polling error: {Error}", errorMessage);
        return Task.CompletedTask;
    }
}
