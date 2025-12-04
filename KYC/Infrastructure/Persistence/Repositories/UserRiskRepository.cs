using Application.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRiskRepository(KycDbContext db) : IUserRiskRepository
{
    public async Task<CustomerRisk?> GetByCustomerByCnpAsync(string cnp, CancellationToken ct)
        => await db.Customers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Cnp == cnp, ct);


    public async Task AddAsync(CustomerRisk customerRisk, CancellationToken ct)
    {
        await db.Customers.AddAsync(customerRisk, ct);
    }
}