using System.Data.Common;
using Application.Interfaces.UnitOfWork;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class TransactionBehavior<TReq, TRes>(IUnitOfWork uow)
    : IPipelineBehavior<TReq, TRes>
    where TRes : ResultBase, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken ct)
    {
        var isQuery = request!.GetType()
            .GetInterfaces()
            .Any(i => i.IsGenericType &&
                      i.GetGenericTypeDefinition() == typeof(IQuery<>));

        if (isQuery) return await next();

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
        catch (DbException exception)
        {
            await transaction.RollbackAsync(ct);
            var fail = new TRes();
            fail.Reasons.Add(new Error($"Unexpected error during transaction: {exception.Message}"));
            return fail;
        }
    }
}