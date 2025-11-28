using Application.DTOs;
using Application.Errors;
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
    IUserRepository userRepository,
    IAccountRepository accountRepository,
    IMapper mapper)
    : ICommandHandler<CreateAccountCommand, Result<AccountDto>>
{
    public async Task<Result<AccountDto>> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(command.CustomerId, cancellationToken);
        if (user is null)
            return Result.Fail<AccountDto>(new NotFoundError("Customer not found"));
        if(user.Role != UserRole.Customer)
            return Result.Fail<AccountDto>(new ValidationError("Only customers can create bank accounts"));

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