using Application.DTOs;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries.TransferQueries;

public class GetTransfersByCustomerIdOrStatusQuery(Guid? customerId, TransferStatus? status):IQuery<Result<List<TransferDto>>>
{
    public Guid? CustomerId => customerId;
    public TransferStatus? Status => status;
}
