using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands.UserCommands;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.CommandHandlers.UserCommandHandlers;

public class RegisterUserCommandHandler(
    IUserRepository repository, IPasswordHasher<string> passwordHasher)
    : ICommandHandler<RegisterUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await repository.GetAccountByEmailAsync(command.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Fail<UserDto>(new ConflictError("Email already in use."));
        }
        
        // Create new user
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = command.Email,
            Role = command.Role,
            Name = command.Name,
            CreatedAt = DateTime.UtcNow,
            PhoneNumber = command.PhoneNumber,
            Country = command.Country,
            Birthday = command.Birthday,
            Address = command.Address,
            Password = passwordHasher.HashPassword(command.Email, command.Password)
        };
        
        // Save user to repository
        await repository.AddAsync(user, cancellationToken);
        
        var userDto = new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Name = user.Name,
            CreatedAt = user.CreatedAt,
            PhoneNumber = user.PhoneNumber,
            Country = user.Country,
            Birthday = user.Birthday,
            Address = user.Address,
            Role = user.Role
        };    
        
        return Result.Ok(userDto);
    }
}