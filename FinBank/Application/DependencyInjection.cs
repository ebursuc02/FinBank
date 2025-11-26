
using Application.DTOs;
using Application.Interfaces.Kyc;
using Application.Policies;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using Application.UseCases.QueryHandlers;
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
            .AddScoped<ICommandHandler<CreateTransferCommand, Result>, CreateTransferCommandHandler>()
            .AddScoped<IRiskPolicyEvaluator, StatusRiskPolicyEvaluator>()
            .AddScoped<IRiskContext, RiskContext>()
            .AddScoped<IQueryHandler<GetTransfersQuery, Result<IEnumerable<TransferOverviewDto>>>,  GetTransfersQueryHandler>();
}