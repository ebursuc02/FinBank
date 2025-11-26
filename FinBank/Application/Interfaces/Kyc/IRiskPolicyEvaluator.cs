using Domain.Enums;

namespace Application.Interfaces.Kyc;

public interface IRiskPolicyEvaluator
{
    TransferStatus Evaluate(RiskStatus riskStatus, out string? reason);
}