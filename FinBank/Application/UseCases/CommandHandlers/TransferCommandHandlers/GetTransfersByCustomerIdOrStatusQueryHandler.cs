using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries.TransferQueries;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public class GetTransfersByCustomerIdOrStatusQueryHandler(ITransferRepository repository):IQueryHandler<GetTransfersByCustomerIdOrStatusQuery,Result<List<TransferDto>>>
{
    public async Task<Result<List<TransferDto>>> HandleAsync(GetTransfersByCustomerIdOrStatusQuery query, CancellationToken cancellationToken)
    {
        var transfers = await repository.GetTransfersByCustomerIdOrStatusAsync(query.CustomerId, query.Status, cancellationToken);
        if (transfers.Count == 0)
        {
            return Result.Fail<List<TransferDto>>("No transfers found for the specified customer ID or status.");
        }
        
        var transferDtos = transfers.Select(t => new TransferDto
        {
            TransferId = t.TransferId,
            Amount = t.Amount,
            Currency = t.Currency,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
        }).ToList();
        
        return Result.Ok(transferDtos);
    }
    
}