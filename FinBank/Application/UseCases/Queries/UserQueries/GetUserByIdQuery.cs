using Application.DTOs;
using Domain;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Queries.CustomerQueries;

public class GetUserByIdQuery(Guid customerId) : IQuery<Result<UserDto>>
{
    public Guid UserId => customerId;
}