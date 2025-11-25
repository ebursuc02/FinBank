using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Domain;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public sealed class CreateTransferCommandHandler(
    IRiskContext riskContext, 
    ITransferRepository repository) : ICommandHandler<CreateTransferCommand, Result>
{
    
    public async Task<Result> HandleAsync(CreateTransferCommand cmd, CancellationToken ct)
    {
        if(riskContext.Current is null) return Result.Fail("Risk could not be evaluated.");
        
        var context = riskContext.Current;

        var entity = new Transfer
        {
            TransferId = Guid.NewGuid(),
            FromAccountId = cmd.FromAccountId,
            ToAccountId   = cmd.ToAccountId,
            Amount = decimal.Round(cmd.Amount, 2, MidpointRounding.ToEven),
            Currency = cmd.Currency,
            Status = context.Decision,
            Reason = context.Reason,
            PolicyVersion = context.PolicyVersion,
            CreatedAt = DateTime.UtcNow,
        };
        
        // TODO: accounts update and currency converting if needed

        await repository.AddAsync(entity, ct);
        return Result.Ok();
    }
}

