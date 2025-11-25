using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class AuthorizationBehavior<TReq,TRes>(IAccountRepository repo) : IPipelineBehavior<TReq,TRes>
{
    public Task<TRes> HandleAsync(TReq request, Func<Task<TRes>> next, CancellationToken ct)
    {
        if (request is not CreateTransferCommand cmd) return next();
        var account = repo.GetByIbanAsync(cmd.FromAccountId, ct).Result;
        var ok = account != null && account.CustomerId == cmd.CustomerId && cmd.Amount <= account.Balance;
        return !ok ? throw new UnauthorizedAccessException("Reason: bank account is not owned by user or not sufficient funds.") : next();
    }
}