using Domain;

namespace Application.Interfaces.Repository;

public interface IEmployeeRepository
{
    Task<Employee?> GetAsync(Guid employeeId, CancellationToken ct);
    Task AddAsync(Employee employee, CancellationToken ct);
}