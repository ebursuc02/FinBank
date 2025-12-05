using Application.DTOs;
using Domain;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid userId, CancellationToken ct);
    Task<User?> GetAccountByEmailAsync(string email, CancellationToken ct);
    public Task<bool> GetIfCustomerEmailAlreadyExistsAsync(string email, CancellationToken ct);
    public Task<bool> GetIfCustomerCnpAlreadyExistsAsync(string cnp, CancellationToken ct);
    Task<string?> GetCustomerCnpByIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task DeleteAsync(Guid commandUserId, CancellationToken cancellationToken);
}