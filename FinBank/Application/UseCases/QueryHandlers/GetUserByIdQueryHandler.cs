using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries.CustomerQueries;
using Domain;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers;

public class GetUserByIdQueryHandler(
    IUserRepository repository)
    : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> HandleAsync(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await repository.GetAsync(query.UserId, ct);

        var userDto = new UserDto
        {
            UserId = user!.UserId,
            Email = user.Email,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber,
            Country = user.Country,
            Birthday = user.Birthday,
            Address = user.Address,
            Role = user.Role
        };
        return userDto;
    }
}