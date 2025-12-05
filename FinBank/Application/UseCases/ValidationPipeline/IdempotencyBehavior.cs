using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Services.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.ValidationPipeline;

public class IdempotencyBehavior<TReq, TRes>(
    IIdempotencyRepository repository
) : IPipelineBehavior<TReq, TRes>
    where TRes : ResultBase, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken ct)
    {
        if (request is not IIdempotencyCheckable idempotencyCheckable)
            return await next();

        var record = await repository.GetAsync(idempotencyCheckable.IdempotencyKey, ct);

        if (record is not null) return IdempotencyCachedResultHandler<TRes>.FromRecord(record);
        
        var result = await next();

        if (result.IsFailed) return result;

        var ik = IdempotencyRecordFactory.Build(
            idempotencyCheckable.IdempotencyKey,
            request,
            result);

        await repository.AddAsync(ik, ct);

        return result;
    }
}