using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class DenyTransferCommand(Guid transferId, Guid? reviewerId, string? reason) : ICommand<Result>
{
    public Guid TransferId => transferId;
    public Guid? ReviewId => reviewerId;
    public string? Reason => reason;
}
    
