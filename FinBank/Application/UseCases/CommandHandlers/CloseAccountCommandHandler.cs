using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers
{
    public class CloseAccountCommandHandler(IAccountRepository accountRepository)
        : ICommandHandler<CloseAccountCommand, Result>
    {
        public async Task<Result> HandleAsync(CloseAccountCommand command, CancellationToken cancellationToken)
        {
            var account = await accountRepository.GetByIbanAsync(command.Iban, cancellationToken);
            if (account!.IsClosed)
                return Result.Fail(new ConflictError("Account is already closed"));

            account.IsClosed = true;
            await accountRepository.UpdateAsync(account, cancellationToken);
            return Result.Ok();
        }
    }
}