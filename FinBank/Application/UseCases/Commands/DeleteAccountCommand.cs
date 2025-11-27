using Mediator.Abstractions;
using FluentResults;

namespace Application.UseCases.Commands;

public class DeleteAccountCommand : ICommand<Result>
{
    public Guid CustomerId { get; set; }
    public string AccountIban { get; set; } = string.Empty;
}

