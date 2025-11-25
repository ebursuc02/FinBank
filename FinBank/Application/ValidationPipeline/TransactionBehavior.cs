using Application.Interfaces.UnitOfWork;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class TransactionBehavior<TReq,TRes>(IUnitOfWork uow) : IPipelineBehavior<TReq,TRes>
{
    public Task<TRes> HandleAsync(TReq input, Func<Task<TRes>> next, CancellationToken ct)
    {
        var transaction = uow.BeginTransactionAsync(ct).Result;
        try
        {
            var result = next();
            transaction.CommitAsync(ct);
            return result;
        }
        catch
        {
            transaction.RollbackAsync(ct);
            throw;
        }
    }
}
