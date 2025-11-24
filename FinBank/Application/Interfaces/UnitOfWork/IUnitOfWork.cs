namespace Application.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct);
}