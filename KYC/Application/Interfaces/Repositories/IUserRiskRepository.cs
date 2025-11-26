using Domain;

namespace Application.Interfaces.Repositories;

public interface IUserRiskRepository
{
    Task<CustomerRisk?> GetByCustomerAsync(Guid id, CancellationToken ct);
    Task AddAsync(CustomerRisk customerRisk, CancellationToken ct);
}