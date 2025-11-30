using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands.TransferCommands;
using Application.UseCases.Queries.TransferQueries;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public class GetTransfersByStatusQueryHandler(ITransferRepository repository):IQueryHandler<
    GetTransfersByStatusQuery, Result<List<TransferDto>>>
{
    public async Task<Result<List<TransferDto>>> HandleAsync(GetTransfersByStatusQuery query, CancellationToken cancellationToken)
    {
        var transfers = await repository.GetAccountsByStatus(query.Status, cancellationToken);

        if (transfers.Count == 0)
        {
            return Result.Fail<List<TransferDto>>("No transfers found with the specified status.");
        }
        
        var transferDto = transfers.Select(transfer => new TransferDto
        {
            TransferId = transfer.TransferId,
            Amount = transfer.Amount,
            Currency = transfer.Currency,
            Status = transfer.Status,
            CreatedAt = transfer.CreatedAt,
        }).ToList();
        
        return Result.Ok(transferDto);
    }
}