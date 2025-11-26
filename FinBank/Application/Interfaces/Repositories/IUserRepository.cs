using Domain;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid userId, CancellationToken ct);
    Task<User?> GetAccountAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}