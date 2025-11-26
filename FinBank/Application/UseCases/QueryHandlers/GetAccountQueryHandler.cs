using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers;

public class GetAccountQueryHandler(
    IAccountRepository repository,
    IMapper mapper
) : IQueryHandler<GetAccountQuery, Result<AccountDto>>
{
    public async Task<Result<AccountDto>> HandleAsync(
        GetAccountQuery query,
        CancellationToken ct)
    {
        var account = await repository.GetByIbanAsync(query.AccountIban, ct);
        return account is null
            ? Result.Fail<AccountDto>("Account not found")
            : Result.Ok(mapper.Map<AccountDto>(account));
    }
}