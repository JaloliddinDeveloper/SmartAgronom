using AqlliAgronom.Infrastructure.Telegram;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
    IConfiguration configuration,
    ILogger<TelegramWebhookController> logger)
    : ControllerBase
{
    // Telegram retries delivery until it receives HTTP 200.
    // This endpoint must ALWAYS return 200 — errors are logged internally.
    [HttpPost("webhook")]
    public async Task<IActionResult> Post(
        [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string? secretToken,
        CancellationToken ct)
    {
        var expectedSecret = configuration["Telegram:WebhookSecret"];
        if (!string.IsNullOrWhiteSpace(expectedSecret) && secretToken != expectedSecret)
            return Unauthorized();

        Update? update;
        try
        {
            // Read raw body and deserialize manually to avoid model-binding failures
            // that cause ASP.NET Core to return 400 before the controller runs.
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync(ct);
            update = JsonSerializer.Deserialize<Update>(json,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to deserialize Telegram webhook body");
            return Ok();
        }

        if (update is null) return Ok();

        try
        {
            await updateHandler.HandleUpdateAsync(botClient, update, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error processing Telegram update {UpdateId}", update.Id);
        }

        return Ok();
    }
}
