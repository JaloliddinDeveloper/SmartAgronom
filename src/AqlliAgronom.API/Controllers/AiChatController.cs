using AqlliAgronom.API.Models;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Application.Features.AiChat.Commands.SendChatMessage;
using AqlliAgronom.Application.Features.AiChat.Commands.StartSession;
using AqlliAgronom.Application.Features.AiChat.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// AI Chat endpoints — requires authentication.
/// </summary>
[Authorize]
public class AiChatController(ICurrentUserService currentUser) : BaseApiController
{
    /// <summary>
    /// Start a new AI consultation session.
    /// </summary>
    [HttpPost("sessions")]
    [ProducesResponseType(typeof(ApiResponse<SessionDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<SessionDto>>> StartSession(
        [FromBody] StartSessionRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;
        var result = await Mediator.Send(new StartSessionCommand(userId, request.TelegramChatId), ct);
        return CreatedResponse(result, $"/api/v1/ai-chat/sessions/{result.Id}");
    }

    /// <summary>
    /// Send a message to the AI and receive an agronomic diagnosis.
    /// </summary>
    [HttpPost("sessions/{sessionId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<ChatResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ChatResponseDto>>> SendMessage(
        Guid sessionId,
        [FromBody] SendMessageRequest request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;
        var result = await Mediator.Send(
            new SendChatMessageCommand(sessionId, request.Message, userId), ct);
        return OkResponse(result);
    }

    /// <summary>
    /// Stream AI response as Server-Sent Events (SSE).
    /// </summary>
    [HttpGet("sessions/{sessionId:guid}/stream")]
    public async Task StreamMessage(
        Guid sessionId, [FromQuery] string message, CancellationToken ct)
    {
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";

        var userId = currentUser.UserId!.Value;

        // For streaming, we send message and write chunks to SSE
        var result = await Mediator.Send(
            new SendChatMessageCommand(sessionId, message, userId), ct);

        // Write the complete response as a single SSE event
        // In production, refactor SendChatMessageCommandHandler to support streaming
        await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(result.Response)}\n\n", ct);
        await Response.WriteAsync("data: [DONE]\n\n", ct);
        await Response.Body.FlushAsync(ct);
    }
}

public record StartSessionRequest(long TelegramChatId);
public record SendMessageRequest(string Message);
