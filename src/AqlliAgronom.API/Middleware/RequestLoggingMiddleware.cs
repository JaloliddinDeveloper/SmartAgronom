using System.Diagnostics;

namespace AqlliAgronom.API.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;

        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();
            var level = context.Response.StatusCode >= 500
                ? LogLevel.Error
                : context.Response.StatusCode >= 400
                    ? LogLevel.Warning
                    : LogLevel.Information;

            logger.Log(level,
                "[{CorrelationId}] {Method} {Path} → {StatusCode} in {ElapsedMs}ms",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }
    }
}
