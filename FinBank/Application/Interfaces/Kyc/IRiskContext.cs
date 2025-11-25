using Domain.Enums;

namespace Application.Interfaces.Kyc;

public sealed record RiskContextData(TransferStatus Decision, string? Reason, string PolicyVersion);

public interface IRiskContext
{
    RiskContextData? Current { get; set; }
    void Clear();
}