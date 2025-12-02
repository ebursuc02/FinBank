using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers;

public class GetTransfersQueryHandler(
    ITransferRepository transferRepository,
    IMapper mapper
) : IQueryHandler<GetTransfersQuery, Result<IEnumerable<TransferDto>>>
{
    public async Task<Result<IEnumerable<TransferDto>>> HandleAsync(
        GetTransfersQuery query,
        CancellationToken ct)
    {
        var transfers = await transferRepository.GetForAccountAsync(
            query.Iban,
            ct);

        var transferList = transfers.ToList();
        var dtoList = transferList.Select(mapper.Map<TransferDto>);

        return Result.Ok(dtoList);
    }
}