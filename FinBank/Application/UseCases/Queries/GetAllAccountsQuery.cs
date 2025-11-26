using Application.DTOs;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries;

public class GetAllAccountsQuery : IQuery<Result<IEnumerable<AccountDto>>>
{
    public Guid CustomerId { get; init; }
}