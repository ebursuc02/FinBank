using Application.UseCases.Commands.TransferCommands;
using FluentResults;
using Mediator.Abstractions;

namespace WebApi.Utils;

public static class TransferUtils
{
    private static IMediator Mediator { get; }

    public static async Task<Result> CompleteOrDenyTransferAsync(Guid transferId, CancellationToken ct)
    {
        var completeResult = await Mediator
            .SendCommandAsync<CompleteTransferCommand, Result>(
                new CompleteTransferCommand { TransferId = transferId }, ct);

        if (completeResult.IsSuccess)
            return completeResult;

        var denyCommand = new DenyTransferCommand(
            transferId,
            string.Join("; ", completeResult.Errors.Select(e => e.Message)));

        var denyResult = await Mediator
            .SendCommandAsync<DenyTransferCommand, Result>(denyCommand, ct);

        return denyResult;
    }
}