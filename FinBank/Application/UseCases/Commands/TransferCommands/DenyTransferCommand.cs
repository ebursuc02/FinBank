using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class DenyTransferCommand(Guid transferId, string? reason) : ICommand<Result>
{
    public Guid TransferId => transferId;
    public string? Reason => reason;
}
    
