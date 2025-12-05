using Domain.Enums;

namespace Domain.Kyc;

public interface IRiskPolicyEvaluator
{
    TransferStatus Evaluate(RiskStatus riskStatus, out string? reason);
}