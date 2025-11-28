using System.Net;
using Application.DTOs;
using Application.Errors;
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
        if (account is null)
            return Result.Fail<AccountDto>(new NotFoundError("Account not found")
                .WithMetadata("StatusCode", HttpStatusCode.NotFound)
            );

        if (account.IsClosed)
            return Result.Fail<AccountDto>(new NotFoundError("Account is closed"));

        return Result.Ok(mapper.Map<AccountDto>(account));
    }
}