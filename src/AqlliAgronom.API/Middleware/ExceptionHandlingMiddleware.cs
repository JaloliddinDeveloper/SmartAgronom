using AqlliAgronom.API.Models;
using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Domain.Common;
using System.Text.Json;

namespace AqlliAgronom.API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var (statusCode, message, errors) = exception switch
        {
            ValidationException ve =>
                (StatusCodes.Status422UnprocessableEntity, "Validation failed.", ve.Errors),

            NotFoundException nfe =>
                (StatusCodes.Status404NotFound, nfe.Message, null),

            UnauthorizedException ue =>
                (StatusCodes.Status401Unauthorized, ue.Message, null),

            ForbiddenException fe =>
                (StatusCodes.Status403Forbidden, fe.Message, null),

            ConflictException ce =>
                (StatusCodes.Status409Conflict, ce.Message, null),

            DomainException de =>
                (StatusCodes.Status400BadRequest, de.Message, null),

            _ => (StatusCodes.Status500InternalServerError,
                  "An unexpected error occurred. Please try again later.", null)
        };

        if (statusCode == 500)
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception [{StatusCode}]. TraceId: {TraceId}",
                statusCode, traceId);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            Message = message,
            Errors = errors,
            TraceId = traceId
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOpts));
    }
}
