using System.Text.RegularExpressions;
using System.Windows.Input;
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
    : ICommandHandler<RegisterUserCommand, Result<User>>
{
    public Task<Result<User>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
    
        // Create new user
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = command.Email,
            Name = command.Name,
            PhoneNumber = command.PhoneNumber,
            Country = command.Country,
            Birthday = command.Birthday,
            Address = command.Address,
            CreatedAt = DateTime.UtcNow
        };

        Console.WriteLine(user);
        // Hash the password
        user.Password = passwordHasher.HashPassword(user.Email, command.Password);

        // Save user to repository
        return repository.AddAsync(user, cancellationToken)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    return Result.Fail<User>("Failed to register user.");
                }
                return Result.Ok(user);
            }, cancellationToken);
    }
}