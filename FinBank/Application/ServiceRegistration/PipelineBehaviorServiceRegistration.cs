using Application.ValidationPipeline;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ServiceRegistration
{
    internal static class PipelineBehaviorServiceRegistration
    {
        public static IServiceCollection AddPipelineBehaviors(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(RiskEvaluationBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        }
    }
}
