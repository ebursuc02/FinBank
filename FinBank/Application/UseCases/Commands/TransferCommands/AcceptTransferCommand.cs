using System.Windows.Input;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands;

public class AcceptTransferCommand(Guid transferId):ICommand<Result>
{
    public Guid TransferId => transferId;
    public TransferStatus Status = TransferStatus.Completed;
}