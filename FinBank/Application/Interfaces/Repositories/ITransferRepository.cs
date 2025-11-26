using Domain;

namespace Application.Interfaces.Repositories;

public interface ITransferRepository
{
    Task<Transfer?> GetAsync(Guid transferId, CancellationToken ct);
    Task AddAsync(Transfer transfer, CancellationToken ct);
    Task<IReadOnlyList<Transfer>> GetForAccountAsync(string iban, int take, int skip, CancellationToken ct);
}