using Application.Errors;
using FluentResults;
using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Commands;
using Domain;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public sealed class CreateTransferCommandHandler(
    IRiskContext riskContext, 
    ITransferRepository transferRepository,
    IBalanceUpdateService balanceUpdateService) : ICommandHandler<CreateTransferCommand, Result>
{
    public async Task<Result> HandleAsync(CreateTransferCommand cmd, CancellationToken ct)
    {
        if(riskContext.Current is null) return Result.Fail(new UnexpectedError("Risk could not be evaluated."));
        var context = riskContext.Current;
        
        var transfer = new Transfer
        {
            TransferId = Guid.NewGuid(),
            FromIban = cmd.Iban,
            ToIban = cmd.ToIban,
            Amount = decimal.Round(cmd.Amount, 2, MidpointRounding.ToEven),
            Currency = cmd.Currency,
            Status = context.Decision,
            Reason = context.Reason,
            PolicyVersion = context.PolicyVersion,
            CreatedAt = DateTime.UtcNow,
        };

        var balanceUpdateResult = await balanceUpdateService.UpdateBalance(transfer, ct);
        if (!balanceUpdateResult.IsSuccess) return balanceUpdateResult;
        
        await transferRepository.AddAsync(transfer, ct);
        return Result.Ok();
    }
}
