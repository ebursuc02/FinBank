using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands.UserCommands;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.CommandHandlers;

public class LoginUserCommandHandler(
    IUserRepository repository,
    IPasswordHasher<string> passwordHasher)
    : ICommandHandler<LoginUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await repository.GetAccountByEmailAsync(command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Fail<UserDto>(new NotFoundError("Invalid email or password."));
        }

        var verification = passwordHasher.VerifyHashedPassword(
            user.Email,
            user.Password,
            command.Password
        );

        if (verification == PasswordVerificationResult.Failed)
            return Result.Fail<UserDto>(new UnauthorizedError("Invalid email or password."));

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