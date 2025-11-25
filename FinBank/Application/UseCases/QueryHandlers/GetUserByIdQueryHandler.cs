using Application.Interfaces.Repositories;
using Application.UseCases.Queries.CustomerQueries;
using Domain;
using Mediator.Abstractions;

namespace Application.UseCases.QueryHandlers;

public class GetUserByIdQueryHandler(
    IUserRepository repository)
    : IQueryHandler<GetUserByIdQuery, User?>
{
    public async Task<User?> HandleAsync(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await repository.GetAsync(query.UserId, ct);
        return user;
    }
}