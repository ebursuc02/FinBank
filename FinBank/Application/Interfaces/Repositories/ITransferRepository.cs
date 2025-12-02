using Domain;
using Domain.Enums;
using FluentResults;

namespace Application.Interfaces.Repositories;

public interface ITransferRepository
{
    Task<Transfer?> GetAsync(Guid transferId, CancellationToken ct);
    Task AddAsync(Transfer transfer, CancellationToken ct);
    Task<IReadOnlyList<Transfer>> GetForAccountAsync(string iban, CancellationToken ct);
    Task<List<Transfer>> GetAccountsByStatus(TransferStatus?  status, CancellationToken ct);
    Task<Result> AcceptTransferAsync(Guid transferId, CancellationToken ct);
    Task<Result> DenyTransferAsync(Guid transferId, string? reason, CancellationToken ct);
    Task<List<Transfer>> GetTransfersByCustomerIdOrStatusAsync(Guid? queryCustomerId, TransferStatus? queryStatus, CancellationToken cancellationToken);
}