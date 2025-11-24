using Application.Interfaces.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AccountRepository(FinBankDbContext db) : IAccountRepository
{
    public async Task<Account?> GetByIbanAsync(string iban, CancellationToken ct = default)
        => await db.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.IBan == iban, ct);


    public async Task<IReadOnlyList<Account>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default)
        => await db.Accounts.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(ct);


    public async Task AddAsync(Account account, CancellationToken ct = default)
    {
        await db.Accounts.AddAsync(account, ct);
    }
}