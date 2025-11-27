using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.Commands;
using AutoMapper;
using Domain;
using Mediator.Abstractions;
using FluentResults;

namespace Application.UseCases.CommandHandlers;

public class CreateAccountCommandHandler(
    IIbanGenerator ibanGenerator,
    IAccountRepository accountRepository,
    IMapper mapper)
    : ICommandHandler<CreateAccountCommand, Result<AccountDto>>
{
    public async Task<Result<AccountDto>> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var iban = ibanGenerator.Generate(command.CustomerId);

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