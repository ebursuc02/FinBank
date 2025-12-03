using Application.DTOs;
using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries;

public class GetTransferByIdQuery : IQuery<Result<TransferDto>>, IAuthorizable, IAccountClosedCheckable
{
    public Guid TransferId { get; init; }
    public Guid CustomerId { get; init; }
    public string Iban { get; init; } = string.Empty;
}