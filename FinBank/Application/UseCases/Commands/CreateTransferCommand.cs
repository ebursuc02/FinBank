using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands;

public sealed class CreateTransferCommand : ICommand<Result>
{
    public required Guid CustomerId { get; init; }
    public required string FromAccountId { get; init; }
    public required string ToAccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public string? IdempotencyKey { get; init; }
    public string? PolicyVersion { get; init; }

    public CreateTransferCommand() {}
}