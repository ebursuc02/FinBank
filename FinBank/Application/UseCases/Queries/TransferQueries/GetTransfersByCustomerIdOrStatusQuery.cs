using Application.DTOs;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries.TransferQueries;

public class GetTransfersByCustomerIdOrStatusQuery(Guid? customerId, string iban, TransferStatus? status):IQuery<Result<List<TransferDto>>>
{
    public Guid? CustomerId => customerId;
    public string Iban => iban;
    public TransferStatus? Status => status;
}
