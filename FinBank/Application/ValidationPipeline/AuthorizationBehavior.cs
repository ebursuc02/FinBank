using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class AuthorizationBehavior<TReq, TRes>(
    IAccountRepository repo) : IPipelineBehavior<TReq, TRes>
    where TRes : Result, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken ct)
    {
        if (request is not IAuthorizable req) return await next();
        
        var account = await repo.GetByIbanAsync(req.Iban, ct);

        var ownershipApproved = account is not null && account.CustomerId == req.CustomerId;
        if (ownershipApproved)
            return await next();
        
        var fail = new TRes();
        fail.WithError("Sender account is not owned by the customer.");
        return fail;
    }
}
