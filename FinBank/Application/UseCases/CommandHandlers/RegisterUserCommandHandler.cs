using System.Text.RegularExpressions;
using System.Windows.Input;
using Application.DTOs;
using Microsoft.AspNetCore.Identity;

using Application.Interfaces.Repositories;
using Application.UseCases.Commands.UserCommands;
using FluentResults;
using FluentValidation;
using Domain;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public class RegisterUserCommandHandler(
    IUserRepository repository, IPasswordHasher<string> passwordHasher)
    : ICommandHandler<RegisterUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
    
        // Create new user
        var user = new UserDto
        {
            UserId = Guid.NewGuid(),
            Email = command.Email,
            Name = command.Name,
            PhoneNumber = command.PhoneNumber,
            Country = command.Country,
            Birthday = command.Birthday,
            Address = command.Address,
            Password = passwordHasher.HashPassword(command.Email, command.Password)
        };
        
        // Save user to repository
        await repository.AddAsync(user, cancellationToken);
        
        return Result.Ok(user);
    }
}