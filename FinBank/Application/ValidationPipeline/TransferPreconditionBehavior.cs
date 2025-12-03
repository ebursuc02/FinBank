using Application.Errors;
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
            return Fail(new ForbiddenError("Sender account is not owned by the customer."));
        }
        
        if (request is not IAccountClosedCheckable req || !account!.IsClosed) return await next();

        return Fail(new NotFoundError("Account is closed"));

    }

    private static TRes Fail(BaseApplicationError  error)
    {
        var fail = new TRes();
        fail.Reasons.Add(error);
        return fail;
    }
}