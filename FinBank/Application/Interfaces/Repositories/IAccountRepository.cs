using Domain;

namespace Application.Interfaces.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIbanAsync(string iban, CancellationToken ct);
    Task<IReadOnlyList<Account>> GetByCustomerAsync(Guid customerId, CancellationToken ct);
    Task AddAsync(Account account, CancellationToken ct);
    Task UpdateAsync(Account account, CancellationToken ct);
    Task DeleteAsync(string accountIban, CancellationToken ct);
}