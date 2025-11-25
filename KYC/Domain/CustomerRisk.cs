using Domain.Enums;

namespace Domain;

public class CustomerRisk
{
    public Guid CustomerId { get; set; }
    public RiskStatus RiskStatus { get; set; }
    public DateTime UpdatedAt { get; set; }
}