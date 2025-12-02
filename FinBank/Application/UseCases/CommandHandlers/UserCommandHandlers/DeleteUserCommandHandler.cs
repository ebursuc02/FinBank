using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands.UserCommands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public class DeleteUserCommandHandler(IUserRepository repository):ICommandHandler<DeleteUserCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await repository.GetAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Fail(new NotFoundError("User not found."));
        }
        
        await repository.DeleteAsync(command.UserId, cancellationToken);

        return Result.Ok();
    }
}