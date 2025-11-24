using Domain;

namespace Application.Interfaces.Repository;

public interface ITransferRepository
{
    Task<Transfer?> GetAsync(Guid transferId, CancellationToken ct = default);
    Task AddAsync(Transfer transfer, CancellationToken ct = default);
    Task<IReadOnlyList<Transfer>> GetForAccountAsync(string iban, int take, CancellationToken ct = default);
}