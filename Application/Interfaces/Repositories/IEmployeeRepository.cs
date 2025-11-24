using Domain;

namespace Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetAsync(Guid employeeId, CancellationToken ct);
    Task AddAsync(Employee employee, CancellationToken ct);
}