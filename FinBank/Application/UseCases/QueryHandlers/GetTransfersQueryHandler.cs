using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers;

public class GetTransfersQueryHandler(
    ITransferRepository repository
) : IQueryHandler<GetTransfersQuery, Result<IEnumerable<TransferOverviewDto>>>
{
    public async Task<Result<IEnumerable<TransferOverviewDto>>> HandleAsync(
        GetTransfersQuery query,
        CancellationToken ct)
    {
        const int pageSize = 5;
        const int offset = 0;

        var transfers = await repository.GetForAccountAsync(
            query.AccountIban,
            pageSize,
            offset,
            ct);

        // TODO: refactoring needed
        var dtoList = transfers
            .Select(t =>
            {
                var isOutgoing = string.Equals(
                    t.FromIban,
                    query.AccountIban,
                    StringComparison.OrdinalIgnoreCase);

                var direction = isOutgoing
                    ? TransferDirection.Outgoing
                    : TransferDirection.Ingoing;

                var counterpartyIban = isOutgoing
                    ? t.ToIban
                    : t.FromIban;

                return new TransferOverviewDto
                {
                    TransferId = t.TransferId,
                    TransferDirectionType = direction.ToString(),
                    DisplayedName = counterpartyIban,
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt,
                    Status = t.Status,
                    Amount = t.Amount,
                    Currency = t.Currency
                };
            })
            .ToList() ?? [];
        
        return Result.Ok<IEnumerable<TransferOverviewDto>>(dtoList);
    }
}