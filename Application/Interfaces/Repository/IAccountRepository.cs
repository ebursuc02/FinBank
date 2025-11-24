using Domain;

namespace Application.Interfaces.Repository;

public interface IAccountRepository
{
    Task<Account?> GetByIbanAsync(string iban, CancellationToken ct = default);
    Task<IReadOnlyList<Account>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
}