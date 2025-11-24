using Domain;

namespace Application.Interfaces.Repository;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey?> GetAsync(string key, CancellationToken ct = default);
    Task AddAsync(IdempotencyKey entry, CancellationToken ct = default);
}