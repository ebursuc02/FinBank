using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class AcceptTransferCommand(Guid transferId):ICommand<Result>
{
    public Guid TransferId => transferId;
    public TransferStatus Status = TransferStatus.Completed;
}