using Application.Interfaces.UnitOfWork;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class TransactionBehavior<TReq, TRes>(IUnitOfWork uow)
    : IPipelineBehavior<TReq, TRes>
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
            await uow.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}