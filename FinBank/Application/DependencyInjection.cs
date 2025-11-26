using Application.DTOs;
using Application.Interfaces.Kyc;
using Application.Policies;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.Queries;
using Application.UseCases.Queries.CustomerQueries;
using Application.UseCases.QueryHandlers;
using Application.ValidationPipeline;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Identity;
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
            .AddScoped<IQueryHandler<GetUserByIdQuery, User?>, GetUserByIdQueryHandler>()
            .AddScoped<IPasswordHasher<string>, PasswordHasher<string>>()
            .AddScoped<ICommandHandler<RegisterUserCommand, Result<UserDto>>, RegisterUserCommandHandler>()
            .AddScoped<ICommandHandler<LoginUserCommand, Result<UserDto>>, LoginUserCommandHandler>()
            .AddScoped<ICommandHandler<CreateTransferCommand, Result>, CreateTransferCommandHandler>()
            .AddScoped<IRiskPolicyEvaluator, StatusRiskPolicyEvaluator>()
            .AddScoped<IRiskContext, RiskContext>()
            .AddScoped<IQueryHandler<GetTransfersQuery, Result<IEnumerable<TransferOverviewDto>>>,  GetTransfersQueryHandler>();
}