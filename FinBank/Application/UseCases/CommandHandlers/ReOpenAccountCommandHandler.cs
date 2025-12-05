using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers
{
    public class ReOpenAccountCommandHandler(IAccountRepository accountRepository)
        : ICommandHandler<ReOpenAccountCommand, Result>
    {
        public async Task<Result> HandleAsync(ReOpenAccountCommand command, CancellationToken cancellationToken)
        {
            var account = await accountRepository.GetByIbanAsync(command.Iban, cancellationToken);
            if (!account!.IsClosed)
                return Result.Fail(new ConflictError("Account is already open"));

            account.IsClosed = false;
            await accountRepository.UpdateAsync(account, cancellationToken);
            return Result.Ok();
        }
    }
}