using Domain;

namespace Application.Interfaces.Repositories;

public interface ICustomerRiskRepository
{
    Task<CustomerRisk?> GetByCustomerAsync(Guid id, CancellationToken ct);
    Task AddAsync(CustomerRisk customerRisk, CancellationToken ct);
}