using System.Collections.Concurrent;
using AqlliAgronom.Application.Features.AiChat.Commands.SendChatMessage;
using AqlliAgronom.Application.Features.AiChat.Commands.StartSession;
using AqlliAgronom.Application.Features.Auth.Commands.RegisterViaTelegram;
using AqlliAgronom.Application.Features.Knowledge.Commands.CreateKnowledgeEntry;
using AqlliAgronom.Application.Features.Knowledge.Commands.PublishKnowledgeEntry;
using AqlliAgronom.Application.Features.Orders.Commands.ConfirmOrder;
using AqlliAgronom.Application.Features.Orders.Commands.PlaceOrder;
using AqlliAgronom.Application.Features.Products.Commands.CreateProduct;
using AqlliAgronom.Application.Features.Products.Commands.DeleteProduct;
using AqlliAgronom.Application.Features.Users.Commands.PromoteToAgronom;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUser = Telegram.Bot.Types.User;

namespace AqlliAgronom.Infrastructure.Telegram;

public class TelegramUpdateHandler(
    ITelegramBotClient botClient,
    ISender mediator,
    IUserRepository userRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    ILogger<TelegramUpdateHandler> logger)
    : IUpdateHandler
{
    // ── Per-chat states ───────────────────────────────────────────────────────
    private static readonly ConcurrentDictionary<long, RegistrationState>   _regStates           = new();
    private static readonly ConcurrentDictionary<long, OrderState>          _orderStates          = new();
    private static readonly ConcurrentDictionary<long, AddProductState>     _addProductStates     = new();
    private static readonly ConcurrentDictionary<long, PromoteState>        _promoteStates        = new();
    private static readonly ConcurrentDictionary<long, AddKnowledgeState>   _addKnowledgeStates   = new();

    private record RegistrationState(string Step, string? FullName = null);
    private record OrderState(Guid ProductId, string ProductName);
    private record AddProductState(
        string Step,
        string? Name                = null,
        string? Description         = null,
        string? UsageInstructions   = null,
        decimal Price               = 0);
    private record PromoteState();
    private record AddKnowledgeState(
        string Step,
        string? Crop            = null,
        string? Problem         = null,
        ProblemCategory? Cat    = null,
        string? Symptoms        = null,
        string? Explanation     = null,
        string? Products        = null,
        string? Dosage          = null,
        string? Application     = null);

    // ─────────────────────────────────────────────────────────────────────────

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

    // ── Main message dispatcher ───────────────────────────────────────────────
    private async Task HandleMessageAsync(Message message, CancellationToken ct)
    {
        var chatId = message.Chat.Id;

        // Photo can arrive when agronom is in addproduct photo step
        if (_addProductStates.TryGetValue(chatId, out var aps) && aps.Step == "awaiting_photo")
        {
            if (message.Photo is { Length: > 0 })
            {
                await HandleAddProductPhotoAsync(chatId, message.Photo, aps, message.From, ct);
                return;
            }
            // If text "/skip" — handled below in text flow
        }

        if (message.Text is null) return;

        var text = message.Text.Trim();

        logger.LogInformation("Telegram message from chat {ChatId}: {Text}",
            chatId, text[..Math.Min(80, text.Length)]);

        // ── Active state machines (take priority) ────────────────────────────
        if (_addProductStates.TryGetValue(chatId, out var addState))
        {
            await HandleAddProductStepAsync(chatId, text, message.From, addState, ct);
            return;
        }

        if (_addKnowledgeStates.TryGetValue(chatId, out var akState))
        {
            await HandleAddKnowledgeStepAsync(chatId, text, akState, ct);
            return;
        }

        if (_promoteStates.ContainsKey(chatId))
        {
            await HandlePromotePhoneAsync(chatId, text, ct);
            return;
        }

        if (_orderStates.TryGetValue(chatId, out var orderState))
        {
            await HandleOrderRegionAsync(chatId, text, orderState, ct);
            return;
        }

        if (_regStates.TryGetValue(chatId, out var regState))
        {
            await HandleRegistrationStepAsync(chatId, text, message.From, regState, ct);
            return;
        }

        // ── Global commands ───────────────────────────────────────────────────
        if (text.StartsWith("/cancel"))
        {
            ClearAllStates(chatId);
            await botClient.SendMessage(chatId, "Bekor qilindi.", cancellationToken: ct);
            return;
        }

        if (text.StartsWith("/start"))  { await HandleStartAsync(chatId, ct);      return; }
        if (text.StartsWith("/help"))   { await SendHelpMessageAsync(chatId, ct);   return; }

        // ── Load registered user ──────────────────────────────────────────────
        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (user is null)
        {
            await botClient.SendMessage(chatId,
                "Siz hali ro'yxatdan o'tmagansiz.\n/start — ro'yxatdan o'tish.",
                cancellationToken: ct);
            return;
        }

        // ── Role-based commands ───────────────────────────────────────────────
        var isStaff = user.Role is UserRole.Agronom or UserRole.Admin;
        var isAdmin = user.Role == UserRole.Admin;

        if (text.StartsWith("/addknowledge"))
        {
            if (!isStaff) { await NotifyNotStaffAsync(chatId, ct); return; }
            await StartAddKnowledgeAsync(chatId, ct);
            return;
        }

        if (text.StartsWith("/addproduct"))
        {
            if (!isStaff) { await NotifyNotStaffAsync(chatId, ct); return; }
            await StartAddProductAsync(chatId, ct);
            return;
        }

        if (text.StartsWith("/myproducts"))
        {
            if (!isStaff) { await NotifyNotStaffAsync(chatId, ct); return; }
            await SendMyProductsAsync(chatId, user.Id, ct);
            return;
        }

        if (text.StartsWith("/orders"))
        {
            if (!isStaff) { await NotifyNotStaffAsync(chatId, ct); return; }
            await SendPendingOrdersAsync(chatId, ct);
            return;
        }

        if (text.StartsWith("/promote"))
        {
            if (!isAdmin) { await botClient.SendMessage(chatId, "❌ Faqat admin uchun.", cancellationToken: ct); return; }
            var parts = text.Split(' ', 2);
            if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[1]))
                await PromoteUserAsync(chatId, parts[1].Trim(), ct);
            else
            {
                _promoteStates[chatId] = new PromoteState();
                await botClient.SendMessage(chatId,
                    "Agronomiystga aylantiriladigan foydalanuvchining telefon raqamini yozing:\n_(+998901234567)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
            }
            return;
        }

        // ── Farmer: AI chat ───────────────────────────────────────────────────
        var session = await mediator.Send(new StartSessionCommand(user.Id, chatId), ct);
        await botClient.SendChatAction(chatId, ChatAction.Typing, cancellationToken: ct);

        try
        {
            var response = await mediator.Send(new SendChatMessageCommand(session.Id, text, user.Id), ct);

            await botClient.SendMessage(chatId, response.Response,
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            if (response.SuggestedProductNames.Count > 0)
                await SendProductCardsAsync(chatId, response.SuggestedProductNames, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AI chat error for chat {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "Kechirasiz, xatolik yuz berdi. Qayta urinib ko'ring.",
                cancellationToken: ct);
        }
    }

    // ── /start ────────────────────────────────────────────────────────────────
    private async Task HandleStartAsync(long chatId, CancellationToken ct)
    {
        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (user is not null)
        {
            var roleLabel = user.Role switch
            {
                UserRole.Admin   => "👑 Admin",
                UserRole.Agronom => "👨‍🌾 Agronomiyst",
                _                => "🌱 Dehqon"
            };
            await botClient.SendMessage(chatId,
                $"Xush kelibsiz, *{EscapeMarkdown(user.FullName)}*! {roleLabel}\n\n" +
                "Barcha buyruqlar: /help",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
        else
        {
            _regStates[chatId] = new RegistrationState("awaiting_name");
            await botClient.SendMessage(chatId,
                "Assalomu alaykum! 🌾\n\n" +
                "Men *AqlliAgronom AI* — aqlli agronomiyst yordamchisi.\n\n" +
                "Ro'yxatdan o'tish uchun to'liq ismingizni yozing:\n_(Masalan: Alisher Karimov)_",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
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
                _regStates[chatId] = state with { Step = "awaiting_phone", FullName = text };
                await botClient.SendMessage(chatId,
                    $"Yaxshi, *{EscapeMarkdown(text)}*! 👍\n\nTelefon raqamingizni yozing:\n_(+998901234567)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_phone":
                try
                {
                    await mediator.Send(new RegisterViaTelegramCommand(
                        FullName: state.FullName!, Phone: text,
                        TelegramChatId: chatId, TelegramUsername: from?.Username), ct);

                    _regStates.TryRemove(chatId, out _);

                    await botClient.SendMessage(chatId,
                        $"✅ *Ro'yxatdan o'tdingiz!*\n\n" +
                        $"Ism: *{EscapeMarkdown(state.FullName!)}*\n" +
                        $"Tel: *{EscapeMarkdown(text)}*\n\n" +
                        "Ekinlaringiz muammosini yozing — AI javob beradi.\n/help — buyruqlar",
                        parseMode: ParseMode.Markdown, cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Registration failed for chat {ChatId}", chatId);
                    await botClient.SendMessage(chatId,
                        "❌ " + (ex.Message.Contains("invalid", StringComparison.OrdinalIgnoreCase)
                            ? "Telefon raqam noto'g'ri. Qaytadan yozing (+998901234567):"
                            : "Xatolik. Qayta urinib ko'ring yoki /cancel."),
                        cancellationToken: ct);
                }
                break;
        }
    }

    // ── AddProduct state machine ──────────────────────────────────────────────
    private async Task StartAddProductAsync(long chatId, CancellationToken ct)
    {
        _addProductStates[chatId] = new AddProductState("awaiting_name");
        await botClient.SendMessage(chatId,
            "🌿 *Yangi mahsulot qo'shish*\n\nMahsulot nomini yozing:\n_(Masalan: Aktara 25 WG)_",
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task HandleAddProductStepAsync(
        long chatId, string text, TelegramUser? from, AddProductState state, CancellationToken ct)
    {
        switch (state.Step)
        {
            case "awaiting_name":
                if (text.Length < 2)
                {
                    await botClient.SendMessage(chatId, "Nom juda qisqa. Qaytadan yozing:", cancellationToken: ct);
                    return;
                }
                _addProductStates[chatId] = state with { Step = "awaiting_description", Name = text };
                await botClient.SendMessage(chatId,
                    $"✅ Nom: *{EscapeMarkdown(text)}*\n\n" +
                    "Mahsulot tavsifini yozing (qaysi muammolarga yordam beradi):\n" +
                    "_(Masalan: Bug'doy, arpa, makkajo'xori zararkunandalariga qarshi kuchli insektitsid)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_description":
                if (text.Length < 10)
                {
                    await botClient.SendMessage(chatId,
                        "Tavsif juda qisqa (kamida 10 ta belgi). Qaytadan yozing:",
                        cancellationToken: ct);
                    return;
                }
                _addProductStates[chatId] = state with { Step = "awaiting_usage", Description = text };
                await botClient.SendMessage(chatId,
                    "✅ Tavsif saqlandi.\n\nIshlatish yo'riqnomasini yozing:\n" +
                    "_(Masalan: 10 litr suvga 2 gramm eritib, kechqurun purkang. 7 kunda bir marta.)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_usage":
                if (text.Length < 10)
                {
                    await botClient.SendMessage(chatId,
                        "Yo'riqnoma juda qisqa. Qaytadan yozing:", cancellationToken: ct);
                    return;
                }
                _addProductStates[chatId] = state with { Step = "awaiting_price", UsageInstructions = text };
                await botClient.SendMessage(chatId,
                    "✅ Yo'riqnoma saqlandi.\n\nNarxini yozing (so'mda):\n_(Masalan: 45000)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_price":
                if (!decimal.TryParse(text.Replace(" ", ""), out var price) || price <= 0)
                {
                    await botClient.SendMessage(chatId,
                        "Narx noto'g'ri. Faqat son kiriting (masalan: 45000):",
                        cancellationToken: ct);
                    return;
                }
                _addProductStates[chatId] = state with { Step = "awaiting_photo", Price = price };
                await botClient.SendMessage(chatId,
                    $"✅ Narx: *{price:N0} so'm*\n\nMahsulot rasmini yuboring.\n_(Rasm yo'q bo'lsa /skip yozing)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_photo":
                // text message in photo step = /skip or invalid
                if (text.Equals("/skip", StringComparison.OrdinalIgnoreCase) || text.Equals("skip", StringComparison.OrdinalIgnoreCase))
                    await SaveProductAsync(chatId, state, imageUrl: null, from, ct);
                else
                    await botClient.SendMessage(chatId,
                        "Rasm yuboring yoki /skip yozing (rasmsiz saqlash).",
                        cancellationToken: ct);
                break;
        }
    }

    private async Task HandleAddProductPhotoAsync(
        long chatId, PhotoSize[] photos, AddProductState state, TelegramUser? from, CancellationToken ct)
    {
        // Largest photo = last element
        var fileId   = photos[^1].FileId;
        var imageUrl = $"tg:{fileId}";
        await SaveProductAsync(chatId, state, imageUrl, from, ct);
    }

    private async Task SaveProductAsync(
        long chatId, AddProductState state, string? imageUrl, TelegramUser? from, CancellationToken ct)
    {
        _addProductStates.TryRemove(chatId, out _);

        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (user is null) { await botClient.SendMessage(chatId, "Xatolik: foydalanuvchi topilmadi.", cancellationToken: ct); return; }

        try
        {
            var product = await mediator.Send(new CreateProductCommand(
                Name:               state.Name!,
                Description:        state.Description!,
                UsageInstructions:  state.UsageInstructions!,
                Price:              state.Price,
                Currency:           "UZS",
                ImageUrl:           imageUrl,
                CreatedById:        user.Id), ct);

            var photoStatus = imageUrl != null ? "📸 Rasm: saqlandi" : "📸 Rasm: yo'q";
            await botClient.SendMessage(chatId,
                $"✅ *Mahsulot qo'shildi!*\n\n" +
                $"📦 Nom: *{EscapeMarkdown(product.Name)}*\n" +
                $"💰 Narx: *{product.Price:N0} so'm*\n" +
                $"{photoStatus}\n\n" +
                "Dehqonlarga AI orqali tavsiya qilinadi.\n/myproducts — mahsulotlarim",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save product for chat {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "❌ Mahsulotni saqlashda xatolik. /addproduct — qayta urinib ko'ring.",
                cancellationToken: ct);
        }
    }

    // ── /myproducts ───────────────────────────────────────────────────────────
    private async Task SendMyProductsAsync(long chatId, Guid userId, CancellationToken ct)
    {
        var products = await productRepository.FindAsync(p => p.CreatedById == userId, ct);

        if (products.Count == 0)
        {
            await botClient.SendMessage(chatId,
                "Sizning mahsulotlaringiz yo'q.\n/addproduct — mahsulot qo'shish",
                cancellationToken: ct);
            return;
        }

        await botClient.SendMessage(chatId,
            $"📦 *Sizning mahsulotlaringiz ({products.Count} ta):*",
            parseMode: ParseMode.Markdown, cancellationToken: ct);

        foreach (var p in products.Take(10))
        {
            var status = p.IsAvailable ? "✅ Mavjud" : "❌ Mavjud emas";
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🗑 O'chirish", $"del_product:{p.Id}")
                }
            });

            await botClient.SendMessage(chatId,
                $"*{EscapeMarkdown(p.Name)}*\n" +
                $"💰 {p.Price.Amount:N0} so'm  |  {status}",
                parseMode: ParseMode.Markdown,
                replyMarkup: buttons,
                cancellationToken: ct);
        }
    }

    // ── /orders ───────────────────────────────────────────────────────────────
    private async Task SendPendingOrdersAsync(long chatId, CancellationToken ct)
    {
        var orders = await orderRepository.GetByStatusAsync(Domain.Enums.OrderStatus.Pending, ct);

        if (orders.Count == 0)
        {
            await botClient.SendMessage(chatId, "📭 Hozirda kutilayotgan buyurtmalar yo'q.",
                cancellationToken: ct);
            return;
        }

        await botClient.SendMessage(chatId,
            $"📋 *Yangi buyurtmalar ({orders.Count} ta):*",
            parseMode: ParseMode.Markdown, cancellationToken: ct);

        foreach (var order in orders.Take(10))
        {
            var items = string.Join(", ", order.Items.Select(i => EscapeMarkdown(i.ProductName)));
            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✅ Tasdiqlash", $"confirm_order:{order.Id}"),
                    InlineKeyboardButton.WithCallbackData("❌ Bekor qilish", $"cancel_order:{order.Id}"),
                }
            });

            await botClient.SendMessage(chatId,
                $"🆔 `{order.Id}`\n" +
                $"👤 *{EscapeMarkdown(order.FarmerName)}*\n" +
                $"📞 `{order.FarmerPhone}`\n" +
                $"📍 {EscapeMarkdown(order.Region)}\n" +
                $"📦 {items}\n" +
                $"💰 {order.TotalAmount.Amount:N0} so'm",
                parseMode: ParseMode.Markdown,
                replyMarkup: buttons,
                cancellationToken: ct);
        }
    }

    // ── /promote ─────────────────────────────────────────────────────────────
    private async Task HandlePromotePhoneAsync(long chatId, string phone, CancellationToken ct)
    {
        _promoteStates.TryRemove(chatId, out _);
        await PromoteUserAsync(chatId, phone, ct);
    }

    private async Task PromoteUserAsync(long chatId, string phone, CancellationToken ct)
    {
        try
        {
            var fullName = await mediator.Send(new PromoteToAgronomiystCommand(phone), ct);
            await botClient.SendMessage(chatId,
                $"✅ *{EscapeMarkdown(fullName)}* agronomiystga aylantirилди!\n" +
                $"Telefon: `{EscapeMarkdown(phone)}`\n\n" +
                "Endi u /addproduct va /orders buyruqlaridan foydalana oladi.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            // Notify the promoted user if they're in Telegram
            var promoted = await userRepository.FindByPhoneAsync(phone, ct);
            if (promoted?.TelegramChatId is { } tid)
            {
                await botClient.SendMessage(tid,
                    "🎉 Siz *agronomiyst* sifatida tasdiqlandi!\n\n" +
                    "Yangi buyruqlar:\n" +
                    "/addproduct — mahsulot qo'shish\n" +
                    "/myproducts — mahsulotlarim\n" +
                    "/orders — buyurtmalar",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Promote failed for phone {Phone}", phone);
            await botClient.SendMessage(chatId,
                $"❌ Foydalanuvchi topilmadi: `{EscapeMarkdown(phone)}`\n" +
                "Telefon raqam to'g'ri ekanligini tekshiring.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }

    // ── Farmer order flow ─────────────────────────────────────────────────────
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
                UserId:     user.Id,
                FarmerName: user.FullName,
                Phone:      user.Phone.Value,
                Region:     region,
                Items:      [new OrderItemRequest(state.ProductId, 1)]), ct);

            await botClient.SendMessage(chatId,
                $"✅ *Buyurtma qabul qilindi!*\n\n" +
                $"📦 *{EscapeMarkdown(state.ProductName)}*\n" +
                $"📍 {EscapeMarkdown(region)}\n" +
                $"🆔 `{order.Id}`\n\n" +
                "Agronomiyst tez orada siz bilan bog'lanadi. 🌾",
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            await NotifyAgronomistsAsync(user.FullName, user.Phone.Value, region,
                state.ProductName, order.Id, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Order placement failed for chat {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "❌ Buyurtma berishda xatolik. Qayta urinib ko'ring yoki /cancel.",
                cancellationToken: ct);
        }
    }

    // ── Product cards ─────────────────────────────────────────────────────────
    private async Task SendProductCardsAsync(long chatId, IReadOnlyList<string> productNames, CancellationToken ct)
    {
        foreach (var name in productNames.Take(3))
        {
            var matches = await productRepository.SearchByNameAsync(name, ct);
            var product = matches.FirstOrDefault();

            if (product is null)
            {
                await botClient.SendMessage(chatId,
                    $"💊 *{EscapeMarkdown(name)}* — tavsiya etilgan mahsulot.",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                continue;
            }

            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📋 Batafsil",   $"batafsil:{product.Id}"),
                    InlineKeyboardButton.WithCallbackData("🌿 Ishlatish",  $"ishlatish:{product.Id}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🛒 Buyurtma berish", $"order:{product.Id}"),
                }
            });

            var caption = $"*{EscapeMarkdown(product.Name)}*\n" +
                          $"{EscapeMarkdown(product.Description)}\n\n" +
                          $"💰 *{product.Price.Amount:N0} {product.Price.Currency}*";

            try
            {
                if (!string.IsNullOrWhiteSpace(product.ImageUrl))
                    await SendProductWithImageAsync(chatId, product.ImageUrl, caption, buttons, ct);
                else
                    await botClient.SendMessage(chatId, caption,
                        parseMode: ParseMode.Markdown, replyMarkup: buttons, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed sending product card for {Name}", product.Name);
                await botClient.SendMessage(chatId, caption,
                    parseMode: ParseMode.Markdown, replyMarkup: buttons, cancellationToken: ct);
            }
        }
    }

    private async Task SendProductWithImageAsync(
        long chatId, string imageUrl, string caption, InlineKeyboardMarkup buttons, CancellationToken ct)
    {
        InputFile photo = imageUrl.StartsWith("tg:")
            ? InputFile.FromFileId(imageUrl[3..])
            : InputFile.FromUri(imageUrl);

        await botClient.SendPhoto(chatId, photo, caption: caption,
            parseMode: ParseMode.Markdown, replyMarkup: buttons, cancellationToken: ct);
    }

    // ── Callback query handler ────────────────────────────────────────────────
    private async Task HandleCallbackQueryAsync(CallbackQuery query, CancellationToken ct)
    {
        await botClient.AnswerCallbackQuery(query.Id, cancellationToken: ct);

        var chatId = query.Message!.Chat.Id;
        var data   = query.Data ?? string.Empty;

        if (data.StartsWith("kcat:") && int.TryParse(data[5..], out var catVal))
        {
            await HandleKnowledgeCategoryAsync(chatId, (ProblemCategory)catVal, ct);
        }
        else if (data.StartsWith("batafsil:") && Guid.TryParse(data[9..], out var bId))
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
        else if (data.StartsWith("confirm_order:") && Guid.TryParse(data[14..], out var coId))
        {
            await ConfirmOrderAsync(chatId, coId, ct);
        }
        else if (data.StartsWith("cancel_order:") && Guid.TryParse(data[13..], out var cancelId))
        {
            await CancelOrderMessageAsync(chatId, cancelId, ct);
        }
        else if (data.StartsWith("del_product:") && Guid.TryParse(data[12..], out var dpId))
        {
            await DeleteProductAsync(chatId, dpId, ct);
        }
    }

    private async Task SendProductDetailAsync(long chatId, Guid productId, CancellationToken ct)
    {
        var p = await productRepository.GetByIdAsync(productId, ct);
        if (p is null) { await botClient.SendMessage(chatId, "Mahsulot topilmadi.", cancellationToken: ct); return; }

        var text = $"*{EscapeMarkdown(p.Name)}*\n\n" +
                   $"📝 {EscapeMarkdown(p.Description)}\n";
        if (!string.IsNullOrWhiteSpace(p.Manufacturer))
            text += $"\n🏭 *Ishlab chiqaruvchi:* {EscapeMarkdown(p.Manufacturer)}";
        if (!string.IsNullOrWhiteSpace(p.ActiveIngredient))
            text += $"\n🔬 *Faol modda:* {EscapeMarkdown(p.ActiveIngredient)}";
        if (!string.IsNullOrWhiteSpace(p.Category))
            text += $"\n🗂 *Kategoriya:* {EscapeMarkdown(p.Category)}";
        text += $"\n\n💰 *{p.Price.Amount:N0} {p.Price.Currency}*";

        await botClient.SendMessage(chatId, text, parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task SendProductUsageAsync(long chatId, Guid productId, CancellationToken ct)
    {
        var p = await productRepository.GetByIdAsync(productId, ct);
        if (p is null) { await botClient.SendMessage(chatId, "Mahsulot topilmadi.", cancellationToken: ct); return; }

        await botClient.SendMessage(chatId,
            $"*{EscapeMarkdown(p.Name)} — Ishlatish yo'riqnomasi*\n\n{EscapeMarkdown(p.UsageInstructions)}",
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task StartOrderFlowAsync(long chatId, Guid productId, CancellationToken ct)
    {
        var p = await productRepository.GetByIdAsync(productId, ct);
        if (p is null) { await botClient.SendMessage(chatId, "Mahsulot topilmadi.", cancellationToken: ct); return; }

        if (!p.IsAvailable)
        {
            await botClient.SendMessage(chatId,
                $"❌ *{EscapeMarkdown(p.Name)}* hozirda mavjud emas.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
            return;
        }

        _orderStates[chatId] = new OrderState(p.Id, p.Name);
        await botClient.SendMessage(chatId,
            $"🛒 *{EscapeMarkdown(p.Name)}* uchun buyurtma.\n\nViloyatingizni yozing:\n_(Toshkent, Samarqand, Farg'ona...)_",
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task ConfirmOrderAsync(long chatId, Guid orderId, CancellationToken ct)
    {
        var agronom = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (agronom is null || agronom.Role is not (UserRole.Agronom or UserRole.Admin))
        {
            await botClient.SendMessage(chatId, "❌ Ruxsat yo'q.", cancellationToken: ct);
            return;
        }

        try
        {
            var farmerUserId = await mediator.Send(new ConfirmOrderCommand(orderId, agronom.Id), ct);

            await botClient.SendMessage(chatId,
                $"✅ Buyurtma tasdiqlandi! (`{orderId}`)",
                parseMode: ParseMode.Markdown, cancellationToken: ct);

            // Notify farmer
            var farmer = await userRepository.GetByIdAsync(farmerUserId, ct);
            if (farmer?.TelegramChatId is { } farmerChatId)
            {
                await botClient.SendMessage(farmerChatId,
                    $"🎉 *Buyurtmangiz tasdiqlandi!*\n\n" +
                    $"Agronomiyst: *{EscapeMarkdown(agronom.FullName)}*\n" +
                    $"Tez orada siz bilan bog'lanadi. 🌾",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Confirm order failed {OrderId}", orderId);
            await botClient.SendMessage(chatId, "❌ Tasdiqlashda xatolik.", cancellationToken: ct);
        }
    }

    private async Task CancelOrderMessageAsync(long chatId, Guid orderId, CancellationToken ct)
    {
        // Just inform — full cancellation would need CancelOrderCommand
        await botClient.SendMessage(chatId,
            $"Buyurtma `{orderId}` bekor qilish uchun:\nSabab xabarini yuboring yoki dehqonga qo'ng'iroq qiling.",
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task DeleteProductAsync(long chatId, Guid productId, CancellationToken ct)
    {
        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (user is null) return;

        var product = await productRepository.GetByIdAsync(productId, ct);
        if (product is null) { await botClient.SendMessage(chatId, "Mahsulot topilmadi.", cancellationToken: ct); return; }

        try
        {
            await mediator.Send(new DeleteProductCommand(productId, user.Id), ct);
            await botClient.SendMessage(chatId,
                $"🗑 *{EscapeMarkdown(product.Name)}* o'chirildi.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
        catch (UnauthorizedAccessException)
        {
            await botClient.SendMessage(chatId, "❌ Bu mahsulotni o'chirish uchun ruxsat yo'q.", cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete product failed {ProductId}", productId);
            await botClient.SendMessage(chatId, "❌ O'chirishda xatolik.", cancellationToken: ct);
        }
    }

    // ── Notify agronomists ────────────────────────────────────────────────────
    private async Task NotifyAgronomistsAsync(
        string farmerName, string phone, string region,
        string productName, Guid orderId, CancellationToken ct)
    {
        IReadOnlyList<AqlliAgronom.Domain.Entities.User> agronomists;
        try { agronomists = await userRepository.GetAgronomistsWithTelegramAsync(ct); }
        catch (Exception ex) { logger.LogWarning(ex, "Failed to load agronomists"); return; }

        var notif = $"🔔 *Yangi buyurtma!*\n\n" +
                    $"👤 *{EscapeMarkdown(farmerName)}*\n" +
                    $"📞 `{phone}`\n" +
                    $"📍 {EscapeMarkdown(region)}\n" +
                    $"📦 *{EscapeMarkdown(productName)}*\n" +
                    $"🆔 `{orderId}`";

        var buttons = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("✅ Tasdiqlash", $"confirm_order:{orderId}"),
                InlineKeyboardButton.WithCallbackData("❌ Bekor",      $"cancel_order:{orderId}"),
            }
        });

        foreach (var ag in agronomists)
        {
            try
            {
                await botClient.SendMessage(ag.TelegramChatId!.Value, notif,
                    parseMode: ParseMode.Markdown, replyMarkup: buttons, cancellationToken: ct);
            }
            catch (Exception ex) { logger.LogWarning(ex, "Failed to notify agronom {UserId}", ag.Id); }
        }
    }

    // ── AddKnowledge state machine ────────────────────────────────────────────
    private async Task StartAddKnowledgeAsync(long chatId, CancellationToken ct)
    {
        _addKnowledgeStates[chatId] = new AddKnowledgeState("awaiting_crop");
        await botClient.SendMessage(chatId,
            "🌱 *Yangi bilim qo'shish* (RAG bazasi)\n\n" +
            "*Ekin nomini* yozing:\n_(Masalan: Bug'doy, Pomidor, G'o'za, Makkajo'xori)_",
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task HandleAddKnowledgeStepAsync(
        long chatId, string text, AddKnowledgeState state, CancellationToken ct)
    {
        switch (state.Step)
        {
            case "awaiting_crop":
                if (text.Length < 2) { await botClient.SendMessage(chatId, "Juda qisqa. Ekin nomini yozing:", cancellationToken: ct); return; }
                _addKnowledgeStates[chatId] = state with { Step = "awaiting_problem", Crop = text };
                await botClient.SendMessage(chatId,
                    $"✅ Ekin: *{EscapeMarkdown(text)}*\n\n*Muammo nomini* yozing:\n_(Masalan: Unli shudring, Tripslar, Temir kamchiligi)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_problem":
                if (text.Length < 3) { await botClient.SendMessage(chatId, "Juda qisqa. Muammo nomini yozing:", cancellationToken: ct); return; }
                _addKnowledgeStates[chatId] = state with { Step = "awaiting_category", Problem = text };
                await botClient.SendMessage(chatId,
                    $"✅ Muammo: *{EscapeMarkdown(text)}*\n\nMuammo *kategoriyasini* tanlang:",
                    parseMode: ParseMode.Markdown,
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("🦠 Kasallik",         "kcat:1"),
                                InlineKeyboardButton.WithCallbackData("🐛 Zararkunanda",      "kcat:2") },
                        new[] { InlineKeyboardButton.WithCallbackData("🌿 Begona o't",        "kcat:3"),
                                InlineKeyboardButton.WithCallbackData("🔬 Ozuqa kamchiligi",  "kcat:4") },
                        new[] { InlineKeyboardButton.WithCallbackData("☀️ Abiotik stress",    "kcat:5"),
                                InlineKeyboardButton.WithCallbackData("📋 Boshqa",            "kcat:6") },
                    }),
                    cancellationToken: ct);
                break;

            case "awaiting_symptoms":
                if (text.Length < 10) { await botClient.SendMessage(chatId, "Belgilarni batafsil yozing (kamida 10 belgi):", cancellationToken: ct); return; }
                _addKnowledgeStates[chatId] = state with { Step = "awaiting_explanation", Symptoms = text };
                await botClient.SendMessage(chatId,
                    "✅ Belgilar saqlandi.\n\n*Batafsil tushuntirish* yozing:\n_(Muammo qanday rivojlanadi, qanday sharoitda kuchayadi)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_explanation":
                if (text.Length < 20) { await botClient.SendMessage(chatId, "Tushuntirish juda qisqa. Batafsil yozing:", cancellationToken: ct); return; }
                _addKnowledgeStates[chatId] = state with { Step = "awaiting_products", Explanation = text };
                await botClient.SendMessage(chatId,
                    "✅ Tushuntirish saqlandi.\n\n*Tavsiya etilgan mahsulotlar* nomlarini yozing:\n_(Masalan: Aktara 25WG, Topsin-M, Ridomil Gold)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_products":
                if (text.Length < 3) { await botClient.SendMessage(chatId, "Mahsulot nomlarini yozing:", cancellationToken: ct); return; }
                _addKnowledgeStates[chatId] = state with { Step = "awaiting_dosage", Products = text };
                await botClient.SendMessage(chatId,
                    "✅ Mahsulotlar saqlandi.\n\n*Gektariga dozasi* yozing:\n_(Masalan: 0.2 kg/ga, 1 litr/ga, 2g/10L suv)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_dosage":
                if (text.Length < 2) { await botClient.SendMessage(chatId, "Dozani yozing:", cancellationToken: ct); return; }
                _addKnowledgeStates[chatId] = state with { Step = "awaiting_application", Dosage = text };
                await botClient.SendMessage(chatId,
                    "✅ Doza saqlandi.\n\n*Ishlatish yo'riqnomasi* yozing:\n_(Qachon, qanday, necha marta ishlatish kerak)_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_application":
                if (text.Length < 10) { await botClient.SendMessage(chatId, "Yo'riqnomani batafsil yozing:", cancellationToken: ct); return; }
                _addKnowledgeStates[chatId] = state with { Step = "awaiting_safety", Application = text };
                await botClient.SendMessage(chatId,
                    "✅ Yo'riqnoma saqlandi.\n\n*Ehtiyot choralari* yozing:\n_(Himoya vositalar, hayvonlardan uzoqlashtirish va h.k.)_\n_Yo'q bo'lsa /skip yozing_",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                break;

            case "awaiting_safety":
                var safety = text.Equals("/skip", StringComparison.OrdinalIgnoreCase)
                    ? "Maxsus ehtiyot chorasi yo'q."
                    : text;
                await SaveKnowledgeAsync(chatId, state with { Step = "done" }, safety, ct);
                break;
        }
    }

    private async Task HandleKnowledgeCategoryAsync(long chatId, ProblemCategory category, CancellationToken ct)
    {
        if (!_addKnowledgeStates.TryGetValue(chatId, out var state) || state.Step != "awaiting_category")
            return;

        var catName = category switch
        {
            ProblemCategory.Disease            => "Kasallik",
            ProblemCategory.Pest               => "Zararkunanda",
            ProblemCategory.Weed               => "Begona o't",
            ProblemCategory.NutrientDeficiency => "Ozuqa kamchiligi",
            ProblemCategory.AbiotiStress       => "Abiotik stress",
            _                                  => "Boshqa"
        };

        _addKnowledgeStates[chatId] = state with { Step = "awaiting_symptoms", Cat = category };
        await botClient.SendMessage(chatId,
            $"✅ Kategoriya: *{catName}*\n\n*Belgilar (alomatlar)* yozing:\n_(Dehqon ko'radigan ko'rinishlar: rang o'zgarishi, dog'lar, so'lish...)_",
            parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    private async Task SaveKnowledgeAsync(
        long chatId, AddKnowledgeState state, string safety, CancellationToken ct)
    {
        _addKnowledgeStates.TryRemove(chatId, out _);

        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        if (user is null) { await botClient.SendMessage(chatId, "Xatolik: foydalanuvchi topilmadi.", cancellationToken: ct); return; }

        try
        {
            var title = $"{state.Crop} — {state.Problem}";
            var entry = await mediator.Send(new CreateKnowledgeEntryCommand(
                Title:                  title,
                CropName:               state.Crop!,
                ProblemName:            state.Problem!,
                Category:               state.Cat!.Value,
                Symptoms:               state.Symptoms!,
                DetailedExplanation:    state.Explanation!,
                RecommendedProducts:    state.Products!,
                DosagePerHectare:       state.Dosage!,
                ApplicationInstructions: state.Application!,
                SafetyPrecautions:      safety,
                Language:               Language.Uzbek), ct);

            // Auto-publish so background job picks it up for indexing
            await mediator.Send(new PublishKnowledgeEntryCommand(entry.Id), ct);

            await botClient.SendMessage(chatId,
                $"✅ *Bilim bazasiga qo'shildi!*\n\n" +
                $"🌱 Ekin: *{EscapeMarkdown(state.Crop!)}*\n" +
                $"🔍 Muammo: *{EscapeMarkdown(state.Problem!)}*\n" +
                $"📦 Mahsulotlar: {EscapeMarkdown(state.Products!)}\n\n" +
                "⏳ 5 daqiqa ichida AI bazaga indekslanadi.\n" +
                "Dehqon shu muammo haqida so'rasa, AI sizning ma'lumotingizni ishlatadi.",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SaveKnowledge failed for chat {ChatId}", chatId);
            await botClient.SendMessage(chatId,
                "❌ Saqlashda xatolik. /addknowledge — qayta urinib ko'ring.",
                cancellationToken: ct);
        }
    }

    // ── /help ─────────────────────────────────────────────────────────────────
    private async Task SendHelpMessageAsync(long chatId, CancellationToken ct)
    {
        var user = await userRepository.FindByTelegramChatIdAsync(chatId, ct);
        var isStaff = user?.Role is UserRole.Agronom or UserRole.Admin;
        var isAdmin = user?.Role == UserRole.Admin;

        var help =
            "*🌾 AqlliAgronom AI — Qo'llanma*\n\n" +

            "*📌 Umumiy buyruqlar:*\n" +
            "/start — Botni boshlash\n" +
            "/help — Ushbu qo'llanma\n" +
            "/cancel — Amalni bekor qilish\n\n" +

            "*🌱 Dehqon uchun:*\n" +
            "Muammoni yozing → AI maslahat beradi → mahsulot tavsiya etiladi\n" +
            "📋 Batafsil — mahsulot haqida\n" +
            "🌿 Ishlatish — qo'llash yo'riqnomasi\n" +
            "🛒 Buyurtma berish → viloyat yozing → buyurtma beriladi\n";

        if (isStaff)
        {
            help +=
                "\n*👨‍🌾 Agronomiyst uchun:*\n" +
                "/addproduct — yangi mahsulot qo'shish\n" +
                "  nom → tavsif → yo'riqnoma → narx → rasm\n" +
                "/myproducts — mahsulotlarim ro'yxati\n" +
                "/orders — kutilayotgan buyurtmalar\n" +
                "  ✅ Tasdiqlash tugmasi — buyurtmani tasdiqlash\n\n" +
                "/addknowledge — RAG bazasiga bilim qo'shish\n" +
                "  ekin → muammo → kategoriya → belgilar →\n" +
                "  tushuntirish → mahsulotlar → doza → yo'riqnoma\n" +
                "  _(5 daqiqada AI ishlatib boshlaydi)_\n";
        }

        if (isAdmin)
        {
            help +=
                "\n*👑 Admin uchun:*\n" +
                "/promote +998901234567 — foydalanuvchini agronomiystga aylantirish\n";
        }

        await botClient.SendMessage(chatId, help, parseMode: ParseMode.Markdown, cancellationToken: ct);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private void ClearAllStates(long chatId)
    {
        _regStates.TryRemove(chatId, out _);
        _orderStates.TryRemove(chatId, out _);
        _addProductStates.TryRemove(chatId, out _);
        _addKnowledgeStates.TryRemove(chatId, out _);
        _promoteStates.TryRemove(chatId, out _);
    }

    private async Task NotifyNotStaffAsync(long chatId, CancellationToken ct)
    {
        await botClient.SendMessage(chatId,
            "❌ Bu buyruq faqat agronomiyst va admin uchun.\nRo'yxatdan o'ting va admin sizni tasdiqlaydi.",
            cancellationToken: ct);
    }

    private static string EscapeMarkdown(string text) =>
        text.Replace("_", "\\_").Replace("*", "\\*").Replace("[", "\\[").Replace("`", "\\`");

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
}
