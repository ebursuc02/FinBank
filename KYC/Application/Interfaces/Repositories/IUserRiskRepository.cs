using Domain;

namespace Application.Interfaces.Repositories;

public interface IUserRiskRepository
{
    Task<CustomerRisk?> GetByCustomerByCnpAsync(string cnp, CancellationToken ct);
    Task AddAsync(CustomerRisk customerRisk, CancellationToken ct);
}