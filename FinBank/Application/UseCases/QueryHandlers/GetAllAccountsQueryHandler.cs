using Application.DTOs;
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
        return Result.Ok(accounts.Select(mapper.Map<AccountDto>));
    }
}