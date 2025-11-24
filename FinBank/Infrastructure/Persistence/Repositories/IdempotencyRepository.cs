using Application.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class IdempotencyRepository(FinBankDbContext db) : IIdempotencyRepository
{
    public async Task<IdempotencyKey?> GetAsync(string key, CancellationToken ct)
        => await db.IdempotencyKeys.AsNoTracking().FirstOrDefaultAsync(x => x.IdempotencyKeyValue == key, ct);


    public async Task AddAsync(IdempotencyKey entry, CancellationToken ct)
    {
        await db.IdempotencyKeys.AddAsync(entry, ct);
    }
}