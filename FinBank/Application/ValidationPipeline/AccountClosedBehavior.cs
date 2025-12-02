using Application.Errors;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class AccountClosedBehavior<TReq, TRes>(
    IAccountRepository repo) : IPipelineBehavior<TReq, TRes>
    where TRes : ResultBase, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken ct)
    {
        if (request is not IAuthorizable req) return await next();

        var account = await repo.GetByIbanAsync(req.Iban, ct);
        var fail = new TRes();
        
        // Let AuthorizationBehavior handle not found, it's not our responsibility
        if (account is null)
            return await next();
        
        if (account.IsClosed)
        {
            fail.Reasons.Add(new NotFoundError("Account is closed"));
            return fail;
        }

        return await next();
    }
}