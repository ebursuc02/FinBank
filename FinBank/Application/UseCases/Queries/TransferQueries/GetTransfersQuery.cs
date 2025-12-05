using Application.DTOs;
using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries.TransferQueries;

public class GetTransfersQuery : IQuery<Result<IEnumerable<TransferDto>>>, IAuthorizable, IAccountClosedCheckable
{
    public required Guid CustomerId { get; init; }
    public required string Iban { get; init; }
}