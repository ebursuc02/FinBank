using Application.UseCases.Commands.TransferCommands;
using FluentResults;
using Mediator.Abstractions;

namespace WebApi.Utils;

public static class TransferUtils
{
    public static async Task<Result> CompleteOrDenyTransferAsync(
        IMediator mediator,
        Guid transferId, 
        CancellationToken ct)
    {
        var completeResult = await mediator
            .SendCommandAsync<CompleteTransferCommand, Result>(
                new CompleteTransferCommand { TransferId = transferId }, ct);

        if (completeResult.IsSuccess)
            return completeResult;

        var denyCommand = new DenyTransferCommand(
            transferId: transferId,
            reviewerId: null, // failed transfer use case
            reason: string.Join("; ", completeResult.Errors.Select(e => e.Message)));

        var denyResult = await mediator
            .SendCommandAsync<DenyTransferCommand, Result>(denyCommand, ct);

        return denyResult;
    }
}