using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers;

public class GetTransferByIdQueryHandler(
    ITransferRepository transferRepository,
    IMapper mapper
) : IQueryHandler<GetTransferByIdQuery, Result<TransferDto>>
{
    public async Task<Result<TransferDto>> HandleAsync(GetTransferByIdQuery query, CancellationToken ct)
    {
        var transfer = await transferRepository.GetAsync(query.TransferId, ct);
        return mapper.Map<TransferDto>(transfer);
    }
}