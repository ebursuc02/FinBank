using Application.DTOs;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries;

public class GetAccountQuery : IQuery<Result<AccountDto>>
{
    public string AccountIban { get; init; }
}