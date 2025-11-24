using Domain;

namespace Application.Interfaces.Repository;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey?> GetAsync(string key, CancellationToken ct);
    Task AddAsync(IdempotencyKey entry, CancellationToken ct);
}