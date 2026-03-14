using AqlliAgronom.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AqlliAgronom.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    ICurrentUserService currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int WarningThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > WarningThresholdMs)
        {
            logger.LogWarning(
                "AqlliAgronom Performance: {RequestName} took {ElapsedMs}ms — exceeds {Threshold}ms. User: {UserId}",
                typeof(TRequest).Name,
                sw.ElapsedMilliseconds,
                WarningThresholdMs,
                currentUser.UserId);
        }

        return response;
    }
}
