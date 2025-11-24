using Application.Interfaces.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CustomerRepository(FinBankDbContext db) : ICustomerRepository
{
    public async Task<Customer?> GetAsync(Guid customerId, CancellationToken ct = default)
        => await db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerId == customerId, ct);


    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await db.Customers.AddAsync(customer, ct);
    }
}