using Domain;

namespace Application.Interfaces.Repository;

public interface IEmployeeRepository
{
    Task<Employee?> GetAsync(Guid employeeId, CancellationToken ct = default);
    Task AddAsync(Employee employee, CancellationToken ct = default);
}