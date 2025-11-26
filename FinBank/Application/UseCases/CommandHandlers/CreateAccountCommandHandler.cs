using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using AutoMapper;
using Domain;
using Mediator.Abstractions;
using FluentResults;

namespace Application.UseCases.CommandHandlers;

public class CreateAccountCommandHandler(IAccountRepository accountRepository, IMapper mapper)
    : ICommandHandler<CreateAccountCommand, Result<AccountDto>>
{
    public async Task<Result<AccountDto>> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        // Generate a unique IBAN (simple example, replace with real logic as needed)
        var iban = $"RO{DateTime.UtcNow.Ticks}{command.CustomerId.ToString().Substring(0, 6)}";

        var account = new Account
        {
            Iban = iban,
            CustomerId = command.CustomerId,
            Currency = command.Currency,
            Balance = command.InitialDeposit,
            CreatedAt = DateTime.UtcNow
        };
        await accountRepository.AddAsync(account, cancellationToken);

        return Result.Ok(mapper.Map<AccountDto>(account));
    }
}