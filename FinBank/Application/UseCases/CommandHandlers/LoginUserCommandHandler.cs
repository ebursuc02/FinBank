using Application.Interfaces.Repositories;
using Application.UseCases.Commands.UserCommands;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.CommandHandlers;

public class LoginUserCommandHandler(
    IUserRepository repository, IPasswordHasher<string> passwordHasher)
    :ICommandHandler<LoginUserCommand, Result<User>>
{
    public async Task<Result<User>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await repository.GetAccountAsync(command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Fail<User>("Invalid email or password.");
        }

        var verification = passwordHasher.VerifyHashedPassword(
            user.Email,
            user.Password, 
            command.Password
        );

        return verification == PasswordVerificationResult.Failed ? Result.Fail<User>("Invalid email or password.") : Result.Ok(user);
    }
}