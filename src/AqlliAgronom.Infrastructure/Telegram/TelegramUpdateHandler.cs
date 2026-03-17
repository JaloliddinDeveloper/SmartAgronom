using System.Collections.Concurrent;
using AqlliAgronom.Application.Features.AiChat.Commands.SendChatMessage;
using AqlliAgronom.Application.Features.AiChat.Commands.StartSession;
using AqlliAgronom.Application.Features.Auth.Commands.RegisterViaTelegram;
using AqlliAgronom.Application.Features.Orders.Commands.PlaceOrder;
using AqlliAgronom.Domain.Interfaces.Repositories;
using TelegramUser = Telegram.Bot.Types.User;
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
    IProductRepository productRepository,
    ILogger<TelegramUpdateHandler> logger)
    : IUpdateHandler
{
    // ── Registration state ────────────────────────────────────────────────────
    private static readonly ConcurrentDictionary<long, RegistrationState> _regStates = new();
    private record RegistrationState(string Step, string? FullName = null);

    // ── Order state ───────────────────────────────────────────────────────────
    private static readonly ConcurrentDictionary<long, OrderState> _orderStates = new();
    private record OrderState(Guid ProductId, string ProductName);

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        var task = update.Type switch
        {
            UpdateType.Message       => HandleMessageAsync(update.Message!, ct),
            UpdateType.CallbackQuery => HandleCallbackQueryAsync(update.CallbackQuery!, ct),
            _                        => Task.CompletedTask
        };

        await task;
    }

    private async Task HandleMessageAsync(Message message, CancellationToken ct)
    {
        if (message.Text is null) return;

        var chatId = message.Chat.Id;
        var text   = message.Text.Trim();

        logger.LogInformation("Telegram message from chat {ChatId}: {Text}",
            chatId, text[..Math.Min(100, text.Length)]);

        // ── Order region collection ──────────────────────────────────────────
        if (_orderStates.TryGetValue(chatId, out var orderState))
        {
            await HandleOrderRegionAsync(chatId, text, orderState, ct);
            return;
        }

        // ── Registration state machine (takes priority over other commands) ──
        if (_regStates.TryGetValue(chatId, out var regState))
        {
            await HandleRegistrationStepAsync(chatId, text, message.From, regState, ct);
            return;
        }

        // ── Commands ─────────────────────────────────────────────────────────
        if (text.StartsWith("/start"))
        {
            var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
            if (user is not null)
            {
                await botClient.SendMessage(chatId,
                    $"Xush kelibsiz, {user.FullName}! 🌱\n\n" +
                    "Ekinlaringiz bilan bog'liq muammoni yozing — javob beraman.\n" +
                    "Barcha buyruqlar: /help",
                    cancellationToken: ct);
            }
            else
            {
                _regStates[chatId] = new RegistrationState("awaiting_name");
                await botClient.SendMessage(chatId,
                    "Assalomu alaykum! 🌾\n\n" +
                    "Men *AqlliAgronom AI* — aqlli agronomiyst yordamchisi.\n\n" +
                    "Ro'yxatdan o'tish uchun to'liq ismingizni yozing:\n" +
                    "_(Masalan: Alisher Karimov)_",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
            }
            return;
        }

        if (text.StartsWith("/help"))
        {
            await SendHelpMessageAsync(chatId, ct);
            return;
        }

        if (text.StartsWith("/cancel"))
        {
            _regStates.TryRemove(chatId, out _);
            _orderStates.TryRemove(chatId, out _);
            await botClient.SendMessage(chatId, "Bekor qilindi. /start — qayta boshlash.",
                cancellationToken: ct);
            return;
        }

        // ── Registered user: AI chat ─────────────────────────────────────────
        var registeredUser = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (registeredUser is null)
        {
            await botClient.SendMessage(chatId,
                "Siz hali ro'yxatdan o'tmagansiz.\n/start — ro'yxatdan o'tish.",
                cancellationToken: ct);
            return;
        }

        var session = await mediator.Send(new StartSessionCommand(registeredUser.Id, chatId), ct);
        await botClient.SendChatAction(chatId, ChatAction.Typing, cancellationToken: ct);

        try
        {
            var response = await mediator.Send(
                new SendChatMessageCommand(session.Id, text, registeredUser.Id), ct);

            await botClient.SendMessage(chatId, response.Response,
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            if (response.SuggestedProductNames.Count > 0)
                await SendProductCardsAsync(chatId, response.SuggestedProductNames, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing AI response for chat {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "Kechirasiz, xatolik yuz berdi. Iltimos, qayta urinib ko'ring.",
                cancellationToken: ct);
        }
    }

    // ── Product cards with 3 action buttons ─────────────────────────────────
    private async Task SendProductCardsAsync(long chatId, IReadOnlyList<string> productNames, CancellationToken ct)
    {
        foreach (var name in productNames.Take(3))
        {
            var matches = await productRepository.SearchByNameAsync(name, ct);
            var product = matches.FirstOrDefault();

            if (product is null)
            {
                // Product not in DB yet — show text suggestion only
                await botClient.SendMessage(chatId,
                    $"💊 *{EscapeMarkdown(name)}* — tavsiya etilgan mahsulot.",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                continue;
            }

            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📋 Batafsil",       $"batafsil:{product.Id}"),
                    InlineKeyboardButton.WithCallbackData("🌿 Ishlatish",      $"ishlatish:{product.Id}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🛒 Buyurtma berish", $"order:{product.Id}"),
                }
            });

            var caption = $"*{EscapeMarkdown(product.Name)}*\n" +
                          $"{EscapeMarkdown(product.Description)}\n\n" +
                          $"💰 Narx: *{product.Price.Amount:N0} {product.Price.Currency}*";

            try
            {
                if (!string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    await botClient.SendPhoto(chatId,
                        InputFile.FromUri(product.ImageUrl),
                        caption: caption,
                        parseMode: ParseMode.Markdown,
                        replyMarkup: buttons,
                        cancellationToken: ct);
                }
                else
                {
                    await botClient.SendMessage(chatId, caption,
                        parseMode: ParseMode.Markdown,
                        replyMarkup: buttons,
                        cancellationToken: ct);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to send product card for {ProductName}", product.Name);
                await botClient.SendMessage(chatId, caption,
                    parseMode: ParseMode.Markdown,
                    replyMarkup: buttons,
                    cancellationToken: ct);
            }
        }
    }

    // ── Callback query handler ────────────────────────────────────────────────
    private async Task HandleCallbackQueryAsync(CallbackQuery query, CancellationToken ct)
    {
        await botClient.AnswerCallbackQuery(query.Id, cancellationToken: ct);

        var chatId = query.Message!.Chat.Id;
        var data   = query.Data ?? string.Empty;

        if (data.StartsWith("batafsil:") && Guid.TryParse(data[9..], out var bId))
        {
            await SendProductDetailAsync(chatId, bId, ct);
        }
        else if (data.StartsWith("ishlatish:") && Guid.TryParse(data[10..], out var uId))
        {
            await SendProductUsageAsync(chatId, uId, ct);
        }
        else if (data.StartsWith("order:") && Guid.TryParse(data[6..], out var oId))
        {
            await StartOrderFlowAsync(chatId, oId, ct);
        }
    }

    private async Task SendProductDetailAsync(long chatId, Guid productId, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(productId, ct);
        if (product is null) { await botClient.SendMessage(chatId, "Mahsulot topilmadi.", cancellationToken: ct); return; }

        var text = $"*{EscapeMarkdown(product.Name)}*\n\n" +
                   $"📝 *Tavsif:*\n{EscapeMarkdown(product.Description)}\n\n";

        if (!string.IsNullOrWhiteSpace(product.Manufacturer))
            text += $"🏭 *Ishlab chiqaruvchi:* {EscapeMarkdown(product.Manufacturer)}\n";
        if (!string.IsNullOrWhiteSpace(product.ActiveIngredient))
            text += $"🔬 *Faol modda:* {EscapeMarkdown(product.ActiveIngredient)}\n";
        if (!string.IsNullOrWhiteSpace(product.Category))
            text += $"🗂 *Kategoriya:* {EscapeMarkdown(product.Category)}\n";

        text += $"\n💰 *Narx:* {product.Price.Amount:N0} {product.Price.Currency}";

        await botClient.SendMessage(chatId, text, parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task SendProductUsageAsync(long chatId, Guid productId, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(productId, ct);
        if (product is null) { await botClient.SendMessage(chatId, "Mahsulot topilmadi.", cancellationToken: ct); return; }

        var text = $"*{EscapeMarkdown(product.Name)} — Ishlatish yo'riqnomasi*\n\n" +
                   EscapeMarkdown(product.UsageInstructions);

        await botClient.SendMessage(chatId, text, parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task StartOrderFlowAsync(long chatId, Guid productId, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(productId, ct);
        if (product is null) { await botClient.SendMessage(chatId, "Mahsulot topilmadi.", cancellationToken: ct); return; }

        if (!product.IsAvailable)
        {
            await botClient.SendMessage(chatId, $"❌ *{EscapeMarkdown(product.Name)}* hozirda mavjud emas.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
            return;
        }

        _orderStates[chatId] = new OrderState(product.Id, product.Name);

        await botClient.SendMessage(chatId,
            $"🛒 *{EscapeMarkdown(product.Name)}* uchun buyurtma beramiz.\n\n" +
            "Viloyatingizni yozing:\n" +
            "_(Masalan: Toshkent, Samarqand, Farg'ona)_",
            parseMode: ParseMode.Markdown,
            cancellationToken: ct);
    }

    private async Task HandleOrderRegionAsync(long chatId, string region, OrderState state, CancellationToken ct)
    {
        _orderStates.TryRemove(chatId, out _);

        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (user is null)
        {
            await botClient.SendMessage(chatId, "Siz hali ro'yxatdan o'tmagansiz. /start",
                cancellationToken: ct);
            return;
        }

        try
        {
            var order = await mediator.Send(new PlaceOrderCommand(
                UserId:      user.Id,
                FarmerName:  user.FullName,
                Phone:       user.Phone.Value,
                Region:      region,
                Items:       [new OrderItemRequest(state.ProductId, 1)]), ct);

            await botClient.SendMessage(chatId,
                $"✅ *Buyurtma qabul qilindi!*\n\n" +
                $"📦 Mahsulot: *{EscapeMarkdown(state.ProductName)}*\n" +
                $"📍 Viloyat: *{EscapeMarkdown(region)}*\n" +
                $"🆔 Buyurtma №: `{order.Id}`\n\n" +
                "Agronomiyst tez orada siz bilan bog'lanadi. 🌾",
                parseMode: ParseMode.Markdown,
                cancellationToken: ct);

            await NotifyAgronomistsAsync(user.FullName, user.Phone.Value, region, state.ProductName, order.Id, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Order placement failed for chat {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "❌ Buyurtma berishda xatolik. Iltimos, qayta urinib ko'ring yoki /cancel.",
                cancellationToken: ct);
        }
    }

    // ── Notify all agronomists with Telegram ──────────────────────────────────
    private async Task NotifyAgronomistsAsync(
        string farmerName, string phone, string region,
        string productName, Guid orderId, CancellationToken ct)
    {
        IReadOnlyList<AqlliAgronom.Domain.Entities.User> agronomists;
        try { agronomists = await userRepository.GetAgronomistsWithTelegramAsync(ct); }
        catch (Exception ex) { logger.LogWarning(ex, "Failed to load agronomists for notification"); return; }

        var notif = $"🔔 *Yangi buyurtma!*\n\n" +
                    $"👤 Dehqon: *{EscapeMarkdown(farmerName)}*\n" +
                    $"📞 Telefon: `{phone}`\n" +
                    $"📍 Viloyat: *{EscapeMarkdown(region)}*\n" +
                    $"📦 Mahsulot: *{EscapeMarkdown(productName)}*\n" +
                    $"🆔 Buyurtma №: `{orderId}`";

        foreach (var agronom in agronomists)
        {
            try
            {
                await botClient.SendMessage(agronom.TelegramChatId!.Value, notif,
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to notify agronom {UserId}", agronom.Id);
            }
        }
    }

    // ── Registration state machine ────────────────────────────────────────────
    private async Task HandleRegistrationStepAsync(
        long chatId, string text, TelegramUser? from, RegistrationState state, CancellationToken ct)
    {
        switch (state.Step)
        {
            case "awaiting_name":
                if (text.Length < 3)
                {
                    await botClient.SendMessage(chatId,
                        "Ism juda qisqa. To'liq ismingizni yozing (kamida 3 harf):",
                        cancellationToken: ct);
                    return;
                }
                _regStates[chatId] = new RegistrationState("awaiting_phone", FullName: text);
                await botClient.SendMessage(chatId,
                    $"Yaxshi, *{EscapeMarkdown(text)}*! 👍\n\n" +
                    "Endi telefon raqamingizni yozing:\n" +
                    "_(Masalan: +998901234567)_",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
                break;

            case "awaiting_phone":
                try
                {
                    await mediator.Send(new RegisterViaTelegramCommand(
                        FullName:          state.FullName!,
                        Phone:             text,
                        TelegramChatId:    chatId,
                        TelegramUsername:  from?.Username), ct);

                    _regStates.TryRemove(chatId, out _);

                    await botClient.SendMessage(chatId,
                        $"✅ Ro'yxatdan o'tdingiz!\n\n" +
                        $"Ism: *{EscapeMarkdown(state.FullName!)}*\n" +
                        $"Tel: *{EscapeMarkdown(text)}*\n\n" +
                        "Endi ekinlaringiz bilan bog'liq savollarni yozing.\n" +
                        "/help — barcha buyruqlar",
                        parseMode: ParseMode.Markdown,
                        cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Registration failed for chat {ChatId}", chatId);
                    await botClient.SendMessage(chatId,
                        "❌ Xatolik: " + (ex.Message.Contains("invalid", StringComparison.OrdinalIgnoreCase)
                            ? "Telefon raqam noto'g'ri. Qaytadan yozing (+998901234567):"
                            : "Ro'yxatdan o'tishda xatolik. Qayta urinib ko'ring yoki /cancel."),
                        cancellationToken: ct);
                }
                break;
        }
    }

    private async Task SendHelpMessageAsync(long chatId, CancellationToken ct)
    {
        var help =
            "*🌾 AqlliAgronom AI — Qo'llanma*\n\n" +

            "*📌 Buyruqlar:*\n" +
            "/start — Botni boshlash / ro'yxatdan o'tish\n" +
            "/help — Ushbu qo'llanma\n" +
            "/cancel — Amaliyotni bekor qilish\n\n" +

            "*🤖 AI maslahat olish:*\n" +
            "Ekinlaringiz muammosini yozing, masalan:\n" +
            "_\"Bug'doy barglari sariqlanib ketdi, nima qilish kerak?\"_\n" +
            "_\"Pomidor kasalligi — oq dog'lar paydo bo'ldi\"_\n\n" +

            "*🛒 Mahsulot buyurtma:*\n" +
            "AI tavsiya qilgan mahsulot kartochkasida:\n" +
            "• *📋 Batafsil* — mahsulot haqida to'liq ma'lumot\n" +
            "• *🌿 Ishlatish* — qo'llash yo'riqnomasi\n" +
            "• *🛒 Buyurtma berish* — buyurtma berish (viloyat so'raladi)\n\n" +

            "*📝 Ro'yxatdan o'tish:*\n" +
            "/start → ism → telefon raqam\n\n" +

            "*👨‍🌾 Agronomist uchun (admin paneli orqali):*\n" +
            "Mahsulot qo'shish: `POST /api/v1/products` (JWT talab etiladi)\n" +
            "Maydonlar: nom, tavsif, ishlatish yo'riqnomasi, narx, rasm URL\n\n" +

            "*ℹ️ Ma'lumot:*\n" +
            "Bot 24/7 ishlaydi. Buyurtma berilganda agronomistga bildirishnoma ketadi.";

        await botClient.SendMessage(chatId, help, parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken ct)
    {
        var msg = exception switch
        {
            ApiRequestException apiEx => $"Telegram API Error [{apiEx.ErrorCode}]: {apiEx.Message}",
            _ => exception.ToString()
        };
        logger.LogError("Telegram polling error [{Source}]: {Error}", source, msg);
        return Task.CompletedTask;
    }

    private static string EscapeMarkdown(string text) =>
        text.Replace("_", "\\_").Replace("*", "\\*").Replace("[", "\\[").Replace("`", "\\`");
}
