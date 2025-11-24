using Application.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.UnitOfWork;

public class EfUnitOfWorkTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
{
    private bool _completed;

    public async Task CommitAsync(CancellationToken ct)
    {
        if (_completed) return;
        await transaction.CommitAsync(ct).ConfigureAwait(false);
        _completed = true;
    }
    
    public async Task RollbackAsync(CancellationToken ct)
    {
        if (_completed) return;
        await transaction.RollbackAsync(ct).ConfigureAwait(false);
        _completed = true;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (!_completed)
        {
            try
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
            }
            catch(OperationCanceledException exception)
            {
                await Console.Error.WriteLineAsync($"Transaction rollback was canceled: {exception.Message}");
            }
            _completed = true;
        }

        await transaction.DisposeAsync().ConfigureAwait(false);
    }
}