using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public class AcceptTransferCommandHandler(ITransferRepository repository):ICommandHandler<AcceptTransferCommand,Result>
{
    public async Task<Result> HandleAsync(AcceptTransferCommand cmd, CancellationToken ct)
    {
        var transfer = await repository.GetAsync(cmd.TransferId, ct);

        if (transfer is null)
            return Result.Fail(new NotFoundError($"Transfer {cmd.TransferId} not found"));    
        
        if (transfer.Status != TransferStatus.UnderReview)
            return Result.Fail( new ConflictError("Only transfers under review  can be accepted"));

        transfer.ReviewedBy = cmd.ReviewerId;
        transfer.Status = TransferStatus.Pending;
        
        await repository.UpdateAsync(transfer, ct);
        return Result.Ok();
    }
} 