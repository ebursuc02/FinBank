using Domain.Enums;
using Domain.Kyc;

namespace Domain.Policies;

public sealed class RiskContext : IRiskContext
{
    private static readonly AsyncLocal<RiskStatus> Data = new();
    public RiskStatus Current { get => Data.Value; set => Data.Value = value; }
}