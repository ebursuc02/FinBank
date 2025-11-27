using Application.DTOs;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class GetTransferApprovalByStatusQuery(TransferStatus?  status):IQuery<Result<List<TransferDto>>>
{
    public TransferStatus?  Status => status;
}