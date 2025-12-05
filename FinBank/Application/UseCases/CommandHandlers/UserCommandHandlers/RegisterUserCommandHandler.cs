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
    IUserRepository repository,
    IPasswordHasher<string> passwordHasher)
    : ICommandHandler<RegisterUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var emailAlreadyUsed = await repository.GetIfCustomerEmailAlreadyExistsAsync(command.Email, cancellationToken);
        if (emailAlreadyUsed)
            return Result.Fail<UserDto>(new ConflictError("Email already in use."));
        var cnpAlreadyUsed = await repository.GetIfCustomerCnpAlreadyExistsAsync(command.Cnp, cancellationToken);
        if (cnpAlreadyUsed)
            return Result.Fail<UserDto>(new ConflictError("Cnp already in use."));

        // Create new user
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = command.Email,
            Cnp = command.Cnp,
            Role = "Customer",
            Name = command.Name,
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
            Cnp = user.Cnp,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber,
            Country = user.Country,
            Birthday = user.Birthday,
            Address = user.Address,
            Role = user.Role
        };

        return Result.Ok(userDto);
    }
}