using Application.DTOs;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class GetTransferApprovalByStatusCommand(TransferStatus  status):ICommand<Result<List<TransferDto>>>
{
    public TransferStatus  Status => status;
}