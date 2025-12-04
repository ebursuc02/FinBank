using Domain.Enums;

namespace Domain;

public class CustomerRisk
{
    public required string Cnp { get; set; }
    public RiskStatus RiskStatus { get; set; }
    public DateTime UpdatedAt { get; set; }
}