using Application.Errors;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.Services.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.ValidationPipeline;

public sealed class TransferPreconditionBehavior<TReq, TRes>(
    IAccountRepository repo
) : IPipelineBehavior<TReq, TRes>
    where TRes : ResultBase, new()
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
            return new ForbiddenError("Sender account is not owned by the customer.").ToResult<TRes>();
        }

        if (request is IAccountClosedCheckable && account!.IsClosed)
        {
            return new NotFoundError("Account is closed").ToResult<TRes>();
        }
 
        return await next();
    }
}