using Application.Errors;
using FluentResults;
using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Domain;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public sealed class CreateTransferCommandHandler(
    IRiskContext riskContext, 
    ITransferRepository transferRepository,
    IAccountRepository accountRepository) : ICommandHandler<CreateTransferCommand, Result>
{
    public async Task<Result> HandleAsync(CreateTransferCommand cmd, CancellationToken ct)
    {
        var senderAccount = await accountRepository.GetByIbanAsync(cmd.Iban, ct);
        var receiverAccount = await accountRepository.GetByIbanAsync(cmd.ToIban, ct);
        
        if(receiverAccount is null) return Result.Fail(new NotFoundError("Receiver account does not exist.")); 
        if(senderAccount == null) return Result.Fail(new NotFoundError("Sender account does not exist."));
        if(cmd.Amount > senderAccount.Balance) return Result.Fail(new ValidationError("No sufficient funds.")); 
        
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
        
        senderAccount.ApplyTransfer(-transfer.Amount, transfer.Currency);
        receiverAccount.ApplyTransfer(transfer.Amount, transfer.Currency);
        
        await accountRepository.UpdateAsync(senderAccount, ct);
        await accountRepository.UpdateAsync(receiverAccount, ct);
        await transferRepository.AddAsync(transfer, ct);
        return Result.Ok();
    }
}
