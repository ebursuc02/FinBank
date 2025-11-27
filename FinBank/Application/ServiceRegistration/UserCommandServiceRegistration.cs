using Application.DTOs;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Queries;
using Application.UseCases.Queries.CustomerQueries;
using Application.UseCases.QueryHandlers;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ServiceRegistration
{
    internal static class UserCommandServiceRegistration
    {
        public static IServiceCollection AddUserCommands(this IServiceCollection services)
        {
            return services
                .AddScoped<ICommandHandler<RegisterUserCommand, Result<UserDto>>, RegisterUserCommandHandler>()
                .AddScoped<ICommandHandler<LoginUserCommand, Result<UserDto>>, LoginUserCommandHandler>()
                .AddScoped<ICommandHandler<DeleteUserCommand, Result>, DeleteUserCommandHandler>()
                .AddScoped<IQueryHandler<GetUserByIdQuery, Result<UserDto>>, GetUserByIdQueryHandler>()
                .AddScoped<IPasswordHasher<string>, PasswordHasher<string>>();
        }
    }
}

