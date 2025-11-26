using Application.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRiskRepository(KycDbContext db) : IUserRiskRepository
{
    public async Task<CustomerRisk?> GetByCustomerAsync(Guid id, CancellationToken ct)
        => await db.Customers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerId == id, ct);


    public async Task AddAsync(CustomerRisk customerRisk, CancellationToken ct)
    {
        await db.Customers.AddAsync(customerRisk, ct);
    }
}