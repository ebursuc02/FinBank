using Domain.Enums;
using Domain.Kyc;

namespace Domain.Policies;

public sealed class StatusRiskPolicyEvaluator : IRiskPolicyEvaluator
{
    private static readonly IReadOnlyDictionary<RiskStatus, TransferStatus> Map =
        new Dictionary<RiskStatus, TransferStatus>
        {
            [RiskStatus.Low]     = TransferStatus.Pending,
            [RiskStatus.Medium]  = TransferStatus.UnderReview,
            [RiskStatus.High]    = TransferStatus.UnderReview,
            [RiskStatus.Blocked] = TransferStatus.Rejected
        };
    
    public TransferStatus Evaluate(RiskStatus riskStatus, out string? reason)
    {
        var decision = Map.GetValueOrDefault(riskStatus, TransferStatus.Pending);

        reason = riskStatus == RiskStatus.Blocked ? "KYC status is Blocked" : null;
        
        return decision;
    }
}