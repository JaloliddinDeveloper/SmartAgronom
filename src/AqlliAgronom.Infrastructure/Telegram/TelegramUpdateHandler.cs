using AqlliAgronom.Application.Features.AiChat.Commands.SendChatMessage;
using AqlliAgronom.Application.Features.AiChat.Commands.StartSession;
using AqlliAgronom.Domain.Interfaces;
using AqlliAgronom.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
            await botClient.SendMessage(chatId,
                "Siz ro'yxatdan o'tmagansiz. Iltimos, avval ro'yxatdan o'ting: /start",
                cancellationToken: ct);
            return;
        }

        var session = await mediator.Send(new StartSessionCommand(user.Id, chatId), ct);

        await botClient.SendChatAction(chatId, ChatAction.Typing, cancellationToken: ct);

        try
        {
            var response = await mediator.Send(
                new SendChatMessageCommand(session.Id, text, user.Id), ct);

            await botClient.SendMessage(chatId, response.Response,
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            if (response.SuggestedProductNames.Count > 0)
                await SendProductSuggestionsAsync(chatId, response.SuggestedProductNames, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing AI response for chat {ChatId}", chatId);
            await botClient.SendMessage(chatId,
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
            await botClient.SendMessage(chatId,
                $"Xush kelibsiz, {user.FullName}! \xF0\x9F\x8C\xB1\n\n" +
                "Men AqlliAgronom AI — sizning aqlli agronomiyst yordamchingizman.\n" +
                "Ekinlaringiz bilan bog'liq muammolarni yozing va men yordam beraman.",
                cancellationToken: ct);
        }
        else
        {
            await botClient.SendMessage(chatId,
                $"Assalomu alaykum, {firstName}! \xF0\x9F\x8C\xBE\n\n" +
                "Men AqlliAgronom AI — aqlli agronomiyst yordamchisi.\n\n" +
                "Foydalanish uchun avval ro'yxatdan o'ting.",
                cancellationToken: ct);
        }
    }

    private async Task SendHelpMessageAsync(long chatId, CancellationToken ct)
    {
        var helpText = "*AqlliAgronom AI — Qo'llanma*\n\n" +
                       "Men sizga quyidagilarda yordam bera olaman:\n\n" +
                       "*Kasallik va zararkunandalarni aniqlash*\n" +
                       "Ekinlaringiz muammolarini tasvirlab yozing\n\n" +
                       "*Mahsulot tavsiyalari*\n" +
                       "Kimyoviy va biologik preparatlar\n\n" +
                       "*Buyurtma berish*\n" +
                       "Tavsiya etilgan mahsulotlarni bevosita buyurtma bering";

        await botClient.SendMessage(chatId, helpText,
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task SendProductSuggestionsAsync(
        long chatId, IReadOnlyList<string> productNames, CancellationToken ct)
    {
        var buttons = productNames.Take(3)
            .Select(name => new[] { InlineKeyboardButton.WithCallbackData(name, $"product:{name}") })
            .ToArray();

        var keyboard = new InlineKeyboardMarkup(buttons);
        await botClient.SendMessage(chatId, "*Tavsiya etilgan mahsulotlar:*",
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: ct);
    }

    private async Task HandleCallbackQueryAsync(CallbackQuery query, CancellationToken ct)
    {
        await botClient.AnswerCallbackQuery(query.Id, cancellationToken: ct);

        if (query.Data?.StartsWith("product:") == true)
        {
            var productName = query.Data[8..];
            await botClient.SendMessage(query.Message!.Chat.Id,
                $"*{productName}* haqida ma'lumot olish uchun veb-saytimizga tashrif buyuring.",
                cancellationToken: ct);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiEx => $"Telegram API Error [{apiEx.ErrorCode}]: {apiEx.Message}",
            _ => exception.ToString()
        };

        logger.LogError("Telegram error [{Source}]: {Error}", source, errorMessage);
        return Task.CompletedTask;
    }
}
