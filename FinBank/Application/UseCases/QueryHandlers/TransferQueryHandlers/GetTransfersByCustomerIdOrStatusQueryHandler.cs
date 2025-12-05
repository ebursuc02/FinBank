using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries.TransferQueries;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers.TransferQueryHandlers;

public class GetTransfersByCustomerIdOrStatusQueryHandler(ITransferRepository repository):IQueryHandler<GetTransfersByCustomerIdOrStatusQuery,Result<List<TransferDto>>>
{
    public async Task<Result<List<TransferDto>>> HandleAsync(GetTransfersByCustomerIdOrStatusQuery query, CancellationToken cancellationToken)
    {
        var transfers = await repository.GetTransfersByCustomerIdOrStatusAndIbanAsync(query.CustomerId, query.Iban, query.Status, cancellationToken);
        if (transfers.Count == 0)
        {
            return Result.Fail<List<TransferDto>>(new NotFoundError("No transfers found for the given criteria"));
        }
        
        var transfersDto = transfers.Select(t => new TransferDto
        {
            TransferId = t.TransferId,
            Amount = t.Amount,
            Currency = t.Currency,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
        }).ToList();
        
        return Result.Ok(transfersDto);
    }
    
}