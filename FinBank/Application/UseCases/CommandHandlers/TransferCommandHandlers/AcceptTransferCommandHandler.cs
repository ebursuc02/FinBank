using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public class AcceptTransferCommandHandler(ITransferRepository repository):ICommandHandler<AcceptTransferCommand,Result>
{
    public async Task<Result> HandleAsync(AcceptTransferCommand command, CancellationToken cancellationToken)
    {
       await repository.AcceptTransferAsync(command.TransferId, cancellationToken);
       return Result.Ok();
    }
}