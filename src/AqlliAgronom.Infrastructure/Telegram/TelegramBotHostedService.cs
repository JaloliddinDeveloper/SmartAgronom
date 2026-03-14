using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace AqlliAgronom.Infrastructure.Telegram;

public class TelegramBotHostedService(
    ITelegramBotClient botClient,
    TelegramUpdateHandler updateHandler,
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
                await botClient.SetWebhook(
                    url: opts.WebhookUrl,
                    secretToken: opts.WebhookSecret,
                    allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery],
                    cancellationToken: ct);
                logger.LogInformation("Telegram webhook set to {Url}", opts.WebhookUrl);
            }
            return;
        }

        // Long polling mode (development)
        _cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
            DropPendingUpdates = true
        };

        botClient.StartReceiving(
            updateHandler: updateHandler,
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
}
