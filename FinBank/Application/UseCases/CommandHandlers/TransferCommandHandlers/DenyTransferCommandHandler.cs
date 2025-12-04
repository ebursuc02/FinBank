using Application.Interfaces.Repositories;
using Application.UseCases.Commands.TransferCommands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public class DenyTransferCommandHandler(ITransferRepository repository):ICommandHandler<DenyTransferCommand, Result>
{
    public async Task<Result> HandleAsync(DenyTransferCommand command, CancellationToken cancellationToken)
    {
        var result = await repository.DenyTransferAsync(command.TransferId, command.ReviewerId ,command.Reason, cancellationToken);
        return result;
    }
}