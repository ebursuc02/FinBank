using System.Data.Common;
using Application.Interfaces.UnitOfWork;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class TransactionBehavior<TReq, TRes>(IUnitOfWork uow)
    : IPipelineBehavior<TReq, TRes>
    where TRes : Result, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken ct = default)
    {
        await using var transaction = await uow.BeginTransactionAsync(ct);

        try
        {
            var result = await next();
            
            if (result.IsSuccess)
            {
                await uow.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return result;
            }
            
            await transaction.RollbackAsync(ct);
            return result;
        }
        catch(DbException exception)
        {
            await transaction.RollbackAsync(ct);
            var fail = new TRes();
            fail.WithError($"Unexpected error during transaction: {exception.Message}");
            return fail;
        }
    }
}