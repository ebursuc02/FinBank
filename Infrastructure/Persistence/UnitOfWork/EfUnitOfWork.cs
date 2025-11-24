using Application.Interfaces.UnitOfWork;

namespace Infrastructure.Persistence.UnitOfWork;

public class EfUnitOfWork(FinBankDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var transaction = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        return new EfUnitOfWorkTransaction(transaction);
    }
}