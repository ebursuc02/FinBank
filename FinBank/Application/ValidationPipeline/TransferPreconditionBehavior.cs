using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class TransferPreconditionBehavior<TReq, TRes>(
    IAccountRepository repo
) : IPipelineBehavior<TReq, TRes>
    where TRes : Result, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken ct)
    {
        if (request is not IAuthorizable authorizable) return await next();
        var account = await repo.GetByIbanAsync(authorizable.Iban, ct);

        var ownershipApproved = account is not null
                                && account.CustomerId == authorizable.CustomerId;

        if (!ownershipApproved)
        {
            return Fail("Sender account is not owned by the customer.");
        }
        
        return await next();
    }

    private static TRes Fail(string errorMessage)
    {
        var result = new TRes();
        result.WithError(errorMessage);
        return result;
    }
}