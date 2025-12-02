using Application.DTOs;
using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries;

public class GetAccountQuery : IQuery<Result<AccountDto>>, IAuthorizable, IAccountClosedCheck
{
    public Guid CustomerId { get; init; }
    public string Iban { get; init; } = string.Empty;
}