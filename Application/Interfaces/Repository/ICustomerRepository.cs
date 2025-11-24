using Domain;

namespace Application.Interfaces.Repository;

public interface ICustomerRepository
{
    Task<Customer?> GetAsync(Guid customerId, CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
}