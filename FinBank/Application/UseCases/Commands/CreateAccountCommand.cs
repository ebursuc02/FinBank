using Application.DTOs;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands;

public class CreateAccountCommand : ICommand<Result<AccountDto>>
{
    public Guid CustomerId { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal InitialDeposit { get; init; } = 0;
}
