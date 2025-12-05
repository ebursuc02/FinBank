using Domain.Enums;

namespace Domain.Kyc;

public interface IRiskContext
{
    RiskStatus Current { get; set; }
}