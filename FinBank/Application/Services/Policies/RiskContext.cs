using Application.Interfaces.Kyc;

namespace Application.Services.Policies;

public sealed class RiskContext : IRiskContext
{
    private static readonly AsyncLocal<RiskContextData?> Data = new();
    public RiskContextData? Current { get => Data.Value; set => Data.Value = value; }
}