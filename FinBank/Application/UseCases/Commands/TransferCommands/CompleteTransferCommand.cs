using Domain;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands.TransferCommands;

public class CompleteTransferCommand : ICommand<Result>
{
    public Guid TransferId { get; set; }
}