using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public class DenyTransferCommandHandler(ITransferRepository repository):ICommandHandler<DenyTransferCommand, Result>
{
    public async Task<Result> HandleAsync(DenyTransferCommand cmd, CancellationToken ct)
    {
        var transfer = await repository.GetAsync(cmd.TransferId, ct);

        if (transfer is null)
            return Result.Fail(new NotFoundError($"Transfer {cmd.TransferId} not found."));

        if (transfer.Status != TransferStatus.Pending && transfer.Status != TransferStatus.UnderReview)
            return Result.Fail(new ConflictError(
                $"Only pending or under review transfers can be denied."));

        transfer.Status = transfer.Status is TransferStatus.Pending ? TransferStatus.Failed : TransferStatus.Rejected;
        transfer.Reason = cmd.Reason;
        transfer.ReviewedBy ??= cmd.ReviewerId;
        transfer.CompletedAt = DateTime.UtcNow;
        
        await repository.UpdateAsync(transfer, ct);
        return Result.Ok();
    }
}