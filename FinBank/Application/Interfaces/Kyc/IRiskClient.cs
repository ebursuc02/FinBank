using Domain.Enums;
using FluentResults;

namespace Application.Interfaces.Kyc;

public interface IRiskClient
{
    Task<Result<RiskStatus>> GetAsync(Guid customerId, CancellationToken ct);
}
