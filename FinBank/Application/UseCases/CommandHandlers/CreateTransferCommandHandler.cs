using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Domain;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public sealed class CreateTransferCommandHandler(
    IRiskContext riskContext, 
    ITransferRepository repository,
    IAccountRepository accountRepository) : ICommandHandler<CreateTransferCommand, Result>
{
    public async Task<Result> HandleAsync(CreateTransferCommand cmd, CancellationToken ct)
    {
        var senderAccount = accountRepository.GetByIbanAsync(cmd.FromIban, ct).Result;
        var receiverAccount = accountRepository.GetByIbanAsync(cmd.ToIban, ct).Result;
        
        if(receiverAccount is null) return Result.Fail("Receiver account does not exist."); 
        if(cmd.Amount > senderAccount!.Balance) return Result.Fail("No sufficient funds."); 
        
        if(riskContext.Current is null) return Result.Fail("Risk could not be evaluated.");
        var context = riskContext.Current;
        
        var transfer = new Transfer
        {
            TransferId = Guid.NewGuid(),
            FromIban = cmd.FromIban,
            ToIban = cmd.ToIban,
            Amount = decimal.Round(cmd.Amount, 2, MidpointRounding.ToEven),
            Currency = cmd.Currency,
            Status = context.Decision,
            Reason = context.Reason,
            PolicyVersion = context.PolicyVersion,
            CreatedAt = DateTime.UtcNow,
        };
        
        senderAccount!.ApplyTransfer(-transfer.Amount, transfer.Currency);
        receiverAccount.ApplyTransfer(transfer.Amount, transfer.Currency);
        
        await accountRepository.UpdateAsync(senderAccount, ct);
        await accountRepository.UpdateAsync(receiverAccount, ct);
        await repository.AddAsync(transfer, ct);
        return Result.Ok();
    }
}

