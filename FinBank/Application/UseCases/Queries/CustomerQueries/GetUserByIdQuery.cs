using Domain;
using Mediator.Abstractions;

namespace Application.UseCases.Queries.CustomerQueries;

public class GetUserByIdQuery(Guid customerId) : IQuery<User?>
{
    public Guid UserId => customerId;
}