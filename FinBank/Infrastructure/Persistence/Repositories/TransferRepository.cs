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

    public async Task<List<Transfer>> GetAccountsByStatus(TransferStatus? status, CancellationToken ct)
    {
        var query = db.Transfers.AsNoTracking().AsQueryable();
        
        if(status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        
        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AcceptTransferAsync(Guid transferId, CancellationToken ct)
    {
        var transfer = await db.Transfers
            .FirstOrDefaultAsync(x => x.TransferId == transferId, ct);
        
        if (transfer is null)
            throw new InvalidOperationException($"Transfer {transferId} not found.");
        
        if (transfer.Status != TransferStatus.Pending)
            throw new InvalidOperationException(
                $"Only transfers with status '{TransferStatus.Pending}' can be accepted.");
        
        transfer.Status = TransferStatus.Completed;
        transfer.CompletedAt = DateTime.UtcNow;
        
        await db.SaveChangesAsync(ct);
    }
    
    public async Task DenyTransferAsync(Guid transferId, string? reason, CancellationToken ct)
    {
        var transfer = await db.Transfers
            .FirstOrDefaultAsync(x => x.TransferId == transferId, ct);

        if (transfer is null)
            throw new InvalidOperationException($"Transfer {transferId} not found.");

        if (transfer.Status != TransferStatus.Pending)
            throw new InvalidOperationException(
                $"Only transfers with status '{TransferStatus.Pending}' can be denied.");

        transfer.Status = TransferStatus.Rejected;
        transfer.Reason = reason;                     
        transfer.CompletedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }

    public async Task<List<Transfer>> GetTransfersByCustomerIdOrStatusAsync(
        Guid? customerId,
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


        if (status.HasValue)
        {
            var s = status.Value;
            query = query.Where(t => t.Status == s);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

}