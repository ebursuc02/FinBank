using Application.Interfaces.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EmployeeRepository(FinBankDbContext db) : IEmployeeRepository
{
    public async Task<Employee?> GetAsync(Guid employeeId, CancellationToken ct)
        => await db.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == employeeId, ct);


    public async Task AddAsync(Employee employee, CancellationToken ct)
    {
        await db.Employees.AddAsync(employee, ct);
    }
}