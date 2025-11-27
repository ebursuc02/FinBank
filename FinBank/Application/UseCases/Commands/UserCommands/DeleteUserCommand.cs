using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.UserCommands;

public class DeleteUserCommand(Guid userId): ICommand<Result>
{
    public Guid UserId => userId;
}