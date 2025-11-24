using Domain;

namespace Application.Interfaces.Repository;

public interface ICustomerRepository
{
    Task<Customer?> GetAsync(Guid customerId, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
}