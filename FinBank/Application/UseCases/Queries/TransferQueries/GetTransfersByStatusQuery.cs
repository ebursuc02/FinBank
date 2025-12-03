using Application.DTOs;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries.TransferQueries;

public class GetTransfersByStatusQuery(TransferStatus? status):IQuery<Result<List<TransferDto>>>
{
    public TransferStatus?  Status => status;
}