using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public class DeleteAccountCommandHandler(IAccountRepository accountRepository)
    : ICommandHandler<DeleteAccountCommand, Result>
{
    public async Task<Result> HandleAsync
        (DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIbanAsync(command.AccountIban, cancellationToken);
        if (account == null || account.CustomerId != command.CustomerId)
            return Result.Fail("Account not found or does not belong to customer.");

        if (account.Balance != 0)
            return Result.Fail("Account balance must be zero to delete.");

        await accountRepository.DeleteAsync(command.AccountIban, cancellationToken);
        return Result.Ok();
    }
}
