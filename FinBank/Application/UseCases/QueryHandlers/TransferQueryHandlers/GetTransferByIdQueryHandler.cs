using System.Diagnostics;
using Application.DTOs;
using Application.Errors;
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
        if (transfer is null)
            return Result.Fail<TransferDto>(new NotFoundError("Transfer not found"));

        if (transfer.FromIban != query.Iban && transfer.ToIban != query.Iban)
            return Result.Fail<TransferDto>(new UnauthorizedError("Transfer does not belong to the specified account"));

        return Result.Ok(mapper.Map<TransferDto>(transfer));
    }
}