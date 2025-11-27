using Application.DTOs;
using Application.Interfaces.Repositories;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TransferRepository(FinBankDbContext db) : ITransferRepository
{
    public async Task<Transfer?> GetAsync(Guid transferId, CancellationToken ct)
        => await db.Transfers.AsNoTracking().FirstOrDefaultAsync(x => x.TransferId == transferId, ct);


    public async Task AddAsync(Transfer transfer, CancellationToken ct)
    {
        await db.Transfers.AddAsync(transfer, ct);
    }


    public async Task<IReadOnlyList<Transfer>> GetForAccountAsync(string iban, CancellationToken ct)
        => await db.Transfers.AsNoTracking()
            .Where(x => x.FromIban == iban || x.ToIban == iban)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<List<Transfer>> GetAccountsByStatus(TransferStatus status, CancellationToken ct)
    {
        return await db.Transfers
            .AsNoTracking()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }
}