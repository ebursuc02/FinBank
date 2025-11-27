using Application.DTOs;
using Application.Interfaces.Kyc;
using Application.Mapping;
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
    {
        services
            .AddScoped<IMediator, Mediator.Mediator>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RiskEvaluationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
            .AddScoped<IQueryHandler<GetUserByIdQuery, Result<UserDto>>, GetUserByIdQueryHandler>()
            .AddScoped<IPasswordHasher<string>, PasswordHasher<string>>()
            .AddScoped<ICommandHandler<RegisterUserCommand, Result<UserDto>>, RegisterUserCommandHandler>()
            .AddScoped<ICommandHandler<LoginUserCommand, Result<UserDto>>, LoginUserCommandHandler>()
            .AddScoped<ICommandHandler<DeleteUserCommand, Result>, DeleteUserCommandHandler>()
            .AddScoped<ICommandHandler<CreateTransferCommand, Result>, CreateTransferCommandHandler>()
            .AddScoped<ICommandHandler<CreateAccountCommand, Result<AccountDto>>, CreateAccountCommandHandler>()
            .AddScoped<ICommandHandler<DeleteAccountCommand, Result>, DeleteAccountCommandHandler>()
            .AddScoped<IQueryHandler<GetTransfersQuery, Result<IEnumerable<TransferOverviewDto>>>,
                GetTransfersQueryHandler>()
            .AddScoped<IQueryHandler<GetAccountQuery, Result<AccountDto>>, GetAccountQueryHandler>()
            .AddScoped<IQueryHandler<GetAllAccountsQuery, Result<IEnumerable<AccountDto>>>,
                GetAllAccountsQueryHandler>();

        services.AddScoped<IRiskPolicyEvaluator, StatusRiskPolicyEvaluator>();
        services.AddScoped<IRiskContext, RiskContext>();
        services.AddAutoMapper(_ => { }, typeof(ApplicationMappingProfile).Assembly);
        return services;
    }
}