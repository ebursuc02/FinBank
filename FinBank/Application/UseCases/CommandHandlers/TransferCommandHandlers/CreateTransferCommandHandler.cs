using FluentResults;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Domain;
using Domain.Enums;
using Domain.Kyc;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public sealed class CreateTransferCommandHandler(
    IRiskContext riskContext, 
    IRiskPolicyEvaluator evaluator,
    ITransferRepository transferRepository,
    IBalanceUpdateService balanceUpdateService) : ICommandHandler<CreateTransferCommand, Result>
{
    public async Task<Result> HandleAsync(CreateTransferCommand cmd, CancellationToken ct)
    {
        var kycDecisionContext = evaluator.Evaluate(riskContext.Current, out var reason);
        
        var transfer = new Transfer
        {
            TransferId = Guid.NewGuid(),
            FromIban = cmd.Iban,
            ToIban = cmd.ToIban,
            Amount = decimal.Round(cmd.Amount, 2, MidpointRounding.ToEven),
            Currency = cmd.Currency,
            Status = kycDecisionContext,
            Reason = reason,
            PolicyVersion = cmd.PolicyVersion ?? "v1",
            CreatedAt = DateTime.UtcNow,
        };

        if (transfer.Status == TransferStatus.Pending)
        {
            var balanceUpdateResult = await balanceUpdateService.UpdateBalance(transfer, ct);
            if (!balanceUpdateResult.IsSuccess) return balanceUpdateResult;
            transfer.Status = TransferStatus.Completed;
        }
 
        await transferRepository.AddAsync(transfer, ct);
        return Result.Ok();
    }
}
