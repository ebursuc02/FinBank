using Application.DTOs;
using Application.Interfaces.Utils;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries;

public class GetAccountQuery : IQuery<Result<AccountDto>>, IAuthorizable, IAccountClosedCheckable
{
    public Guid CustomerId { get; init; }
    public required string Iban { get; init; } = string.Empty;
}