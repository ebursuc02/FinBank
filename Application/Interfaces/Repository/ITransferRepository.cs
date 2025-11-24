using Domain;

namespace Application.Interfaces.Repository;

public interface ITransferRepository
{
    Task<Transfer?> GetAsync(Guid transferId, CancellationToken ct);
    Task AddAsync(Transfer transfer, CancellationToken ct);
    Task<IReadOnlyList<Transfer>> GetForAccountAsync(string iban, int take, CancellationToken ct);
}