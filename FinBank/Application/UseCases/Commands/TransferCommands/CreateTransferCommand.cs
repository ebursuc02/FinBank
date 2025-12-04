using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class CreateTransferCommand : ICommand<Result<Guid>>, IAuthorizable, IAccountClosedCheckable, IIdempotencyCheckable
{
    public required Guid CustomerId { get; init; }
    public required string Iban { get; init; }
    public required string ToIban { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public Guid IdempotencyKey { get; init; }
    public string? PolicyVersion { get; init; }
}