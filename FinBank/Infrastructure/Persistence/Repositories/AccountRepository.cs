using Application.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AccountRepository(FinBankDbContext db) : IAccountRepository
{
    public async Task<Account?> GetByIbanAsync(string iban, CancellationToken ct)
        => await db.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Iban == iban, ct);


    public async Task<IReadOnlyList<Account>> GetByCustomerAsync(Guid customerId, CancellationToken ct)
        => await db.Accounts.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(ct);


    public async Task AddAsync(Account account, CancellationToken ct)
        => await db.Accounts.AddAsync(account, ct);

    public Task UpdateAsync(Account account, CancellationToken ct)
        => Task.FromResult(db.Accounts.Update(account));
        
}