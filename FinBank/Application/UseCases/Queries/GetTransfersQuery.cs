using Application.DTOs;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries;

public class GetTransfersQuery : IQuery<Result<IEnumerable<TransferOverviewDto>>>
{
    public required Guid CustomerId { get; init; }
    public required string AccountIban { get; init; }
}