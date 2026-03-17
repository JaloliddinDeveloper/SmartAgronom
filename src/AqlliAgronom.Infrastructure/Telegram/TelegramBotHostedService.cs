using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AqlliAgronom.Infrastructure.Telegram;

public class TelegramBotHostedService(
    ITelegramBotClient botClient,
    IServiceScopeFactory scopeFactory,
    IOptions<TelegramBotOptions> options,
    ILogger<TelegramBotHostedService> logger)
    : IHostedService
{
    private CancellationTokenSource? _cts;

    public async Task StartAsync(CancellationToken ct)
    {
        var opts = options.Value;

        if (opts.UseWebhook)
        {
            if (!string.IsNullOrWhiteSpace(opts.WebhookUrl))
            {
                try
                {
                    await botClient.SetWebhook(
                        url: opts.WebhookUrl,
                        secretToken: opts.WebhookSecret,
                        allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery],
                        cancellationToken: ct);
                    logger.LogInformation("Telegram webhook set to {Url}", opts.WebhookUrl);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to set Telegram webhook to {Url}. Bot will not receive updates until webhook is registered.", opts.WebhookUrl);
                }
            }
            return;
        }

        // Long polling mode (development) — creates a fresh DI scope per update
        _cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
            DropPendingUpdates = true
        };

        botClient.StartReceiving(
            updateHandler: new ScopePerUpdateHandler(scopeFactory, logger),
            receiverOptions: receiverOptions,
            cancellationToken: _cts.Token);

        var me = await botClient.GetMe(ct);
        logger.LogInformation("Telegram bot @{Username} started (long polling)", me.Username);
    }

    public async Task StopAsync(CancellationToken ct)
    {
        _cts?.Cancel();

        if (options.Value.UseWebhook)
            await botClient.DeleteWebhook(cancellationToken: ct);

        logger.LogInformation("Telegram bot stopped");
    }

    // Resolves a fresh TelegramUpdateHandler scope for every incoming update,
    // ensuring scoped services (repositories, DbContext) are handled correctly.
    private sealed class ScopePerUpdateHandler(
        IServiceScopeFactory scopeFactory,
        ILogger logger) : IUpdateHandler
    {
        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TelegramUpdateHandler>();
            await handler.HandleUpdateAsync(bot, update, ct);
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
    }
}
