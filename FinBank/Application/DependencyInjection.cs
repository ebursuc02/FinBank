
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.Queries.CustomerQueries;
using Application.UseCases.QueryHandlers;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        =>
            services.AddScoped<IMediator, Mediator.Mediator>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddScoped<IQueryHandler<GetUserByIdQuery, User?>, GetUserByIdQueryHandler>()
                .AddScoped<IPasswordHasher<string>, PasswordHasher<string>>()
                .AddScoped<ICommandHandler<RegisterUserCommand, Result<User>>, RegisterUserCommandHandler>()
                .AddScoped<ICommandHandler<LoginUserCommand, Result<User>>, LoginUserCommandHandler>();


}