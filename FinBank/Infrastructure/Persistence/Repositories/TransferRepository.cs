using Application.Errors;
using Application.Interfaces.Repositories;
using Domain;
using Domain.Enums;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TransferRepository(FinBankDbContext db) : ITransferRepository
{
    public async Task<Transfer?> GetAsync(Guid transferId, CancellationToken ct)
        => await db.Transfers.FirstOrDefaultAsync(x => x.TransferId == transferId, ct);


    public async Task AddAsync(Transfer transfer, CancellationToken ct)
        => await db.Transfers.AddAsync(transfer, ct);
    
    public Task UpdateAsync(Transfer transfer, CancellationToken ct)
        => Task.FromResult(db.Transfers.Update(transfer));

    public async Task<IReadOnlyList<Transfer>> GetForAccountAsync(string iban, CancellationToken ct)
        => await db.Transfers.AsNoTracking()
            .Where(x => x.FromIban == iban || x.ToIban == iban)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<List<Transfer>> GetAccountsByStatus(TransferStatus? status, CancellationToken ct)
    {
        var query = db.Transfers.AsNoTracking().AsQueryable();
        
        if(status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        
        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Transfer>> GetTransfersByCustomerIdOrStatusAndIbanAsync(
        Guid? customerId,
        string iban,
        TransferStatus? status,
        CancellationToken ct)
    {
        var query = db.Transfers
            .AsNoTracking()
            .AsQueryable();

        if (customerId.HasValue)
        {
            var id = customerId.Value;

            query = query.Where(t =>
                db.Accounts.Any(a => a.CustomerId == id && a.Iban == t.FromIban) ||
                db.Accounts.Any(a => a.CustomerId == id && a.Iban == t.ToIban));
        }

        if (!string.IsNullOrWhiteSpace(iban))
        {
            query = query.Where(t => t.FromIban == iban || t.ToIban == iban);
        }

        if (!status.HasValue)
            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);
        {
            var s = status.Value;
            query = query.Where(t => t.Status == s);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }
}