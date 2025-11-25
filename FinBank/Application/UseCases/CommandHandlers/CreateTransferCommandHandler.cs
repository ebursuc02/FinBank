using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public sealed class CreateTransferCommandHandler(
    ITransferRepository repository,
    IMediator mediator) : ICommandHandler<CreateTransferCommand, Result>
{
    public async Task<Result> HandleAsync(CreateTransferCommand command, CancellationToken cancellationToken)
    {
        return Result.Ok();
    }
}
