using Application.UseCases.ValidationPipeline;
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
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(TransferPreconditionBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(KycRetrievalBehavior<,>));
        }
    }
}
