using Application.UseCases.Commands.TransferCommands;
using Domain;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface ITransferRepository
{
    Task<Transfer?> GetAsync(Guid transferId, CancellationToken ct);
    Task AddAsync(Transfer transfer, CancellationToken ct);
    Task<IReadOnlyList<Transfer>> GetForAccountAsync(string iban, CancellationToken ct);
    Task<List<Transfer>> GetAccountsByStatus(TransferStatus?  status, CancellationToken ct);
}