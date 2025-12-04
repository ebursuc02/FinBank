using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain;
using FluentResults;

namespace Application.Services.Utils;

public class BalanceUpdateService(IAccountRepository repository) : IBalanceUpdateService
{
    public async Task<Result> UpdateBalance(Transfer transfer, CancellationToken ct)
    {
        var senderAccount = await repository.GetByIbanAsync(transfer.FromIban, ct);
        var receiverAccount = await repository.GetByIbanAsync(transfer.ToIban, ct);
        
        senderAccount!.ApplyTransfer(-transfer.Amount, transfer.Currency);
        receiverAccount?.ApplyTransfer(transfer.Amount, transfer.Currency);
        
        await repository.UpdateAsync(senderAccount, ct);
        if (receiverAccount != null) await repository.UpdateAsync(receiverAccount, ct);
        return  Result.Ok();
    }
}