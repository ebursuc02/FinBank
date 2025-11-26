using Application.Interfaces.UnitOfWork;

namespace Infrastructure.Persistence.UnitOfWork;

public class EfUnitOfWork(KycDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct)
    {
        var transaction = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        return new EfUnitOfWorkTransaction(transaction);
    }
}