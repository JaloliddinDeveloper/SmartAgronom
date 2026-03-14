using AqlliAgronom.Infrastructure.Telegram;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// Receives Telegram webhook updates and dispatches them to the update handler.
/// </summary>
[ApiController]
[Route("api/v1/telegram")]
public class TelegramWebhookController(
    ITelegramBotClient botClient,
    TelegramUpdateHandler updateHandler,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string? secretToken,
        CancellationToken ct)
    {
        var expectedSecret = configuration["Telegram:WebhookSecret"];

        // Validate secret token if configured
        if (!string.IsNullOrWhiteSpace(expectedSecret) && secretToken != expectedSecret)
            return Unauthorized();

        await updateHandler.HandleUpdateAsync(botClient, update, ct);
        return Ok();
    }
}
