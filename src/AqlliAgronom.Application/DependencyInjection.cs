using System.Reflection;
using AqlliAgronom.Application.AI.Interfaces;
using AqlliAgronom.Application.AI.Pipeline;
using AqlliAgronom.Application.AI.Pipeline.Steps;
using AqlliAgronom.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AqlliAgronom.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR — auto-discovers all IRequestHandler<> and INotificationHandler<>
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // FluentValidation — auto-discovers all AbstractValidator<>
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behaviors in ORDER of execution
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // RAG pipeline steps (ordered)
        services.AddTransient<IRagStep, QueryPreprocessingStep>();      // Order 1
        services.AddTransient<IRagStep, VectorRetrievalStep>();         // Order 2
        services.AddTransient<IRagStep, ContextRankingStep>();          // Order 3
        services.AddTransient<IRagStep, ProductInjectionStep>();        // Order 4
        services.AddTransient<IRagStep, PromptAssemblyStep>();          // Order 5
        services.AddTransient<IRagStep, ResponsePostprocessingStep>();  // Order 6

        return services;
    }
}
