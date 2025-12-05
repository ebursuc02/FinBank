using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class AcceptTransferCommand(Guid transferId, Guid reviewerId):ICommand<Result>
{
    public Guid TransferId => transferId;
    public Guid ReviewerId => reviewerId;
    public TransferStatus Status = TransferStatus.Completed;
}