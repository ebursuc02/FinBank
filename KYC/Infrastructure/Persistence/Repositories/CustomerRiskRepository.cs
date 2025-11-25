using Application.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CustomerRiskRepository(KycDbContext db) : ICustomerRiskRepository
{
    public async Task<CustomerRisk?> GetByCustomerAsync(Guid id, CancellationToken ct)
        => await db.Accounts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerId == id, ct);


    public async Task AddAsync(CustomerRisk customerRisk, CancellationToken ct)
    {
        await db.Accounts.AddAsync(customerRisk, ct);
    }
}