using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public class CompleteTransferCommandHandler(
    ITransferRepository transferRepository,
    IBalanceUpdateService balanceUpdateService) : ICommandHandler<CompleteTransferCommand, Result>
{
    public async Task<Result> HandleAsync(CompleteTransferCommand cmd, CancellationToken ct)
    {
        var transfer = await transferRepository.GetAsync(cmd.TransferId, ct);
        
        if (transfer == null) return Result.Fail("Transfer failed.");
        
        if (transfer.Status != TransferStatus.Pending) return  Result.Ok();

        var balanceUpdateResult = await balanceUpdateService.UpdateBalance(transfer, ct);

        transfer.Status = balanceUpdateResult.IsSuccess ? TransferStatus.Completed : TransferStatus.Failed;
        await transferRepository.MarkCompleted(transfer, ct);
        return Result.Ok();
    }
}