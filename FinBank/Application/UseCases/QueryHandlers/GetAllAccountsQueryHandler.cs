using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers;

public class GetAllAccountsQueryHandler(
    IAccountRepository repository,
    IMapper mapper
) : IQueryHandler<GetAllAccountsQuery, Result<IEnumerable<AccountDto>>>
{
    public async Task<Result<IEnumerable<AccountDto>>> HandleAsync(
        GetAllAccountsQuery query,
        CancellationToken ct)
    {
        var accounts = await repository.GetByCustomerAsync(query.CustomerId, ct);
        var validAccounts = accounts
            .Where(account => !account.IsClosed && account.CustomerId != query.CustomerId)
            .ToList();
        if (!validAccounts.Any())
            return Result.Fail<IEnumerable<AccountDto>>(new NotFoundError("No open accounts found for this customer"));
        return Result.Ok(validAccounts.Select(mapper.Map<AccountDto>));
    }
}