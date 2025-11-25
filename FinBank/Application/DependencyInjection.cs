
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using Application.ValidationPipeline;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddScoped<IMediator, Mediator.Mediator>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RiskEvaluationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
            .AddScoped<ICommandHandler<CreateTransferCommand, Result>, CreateTransferCommandHandler>();
}