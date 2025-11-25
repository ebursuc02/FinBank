namespace WebApi.DTOs;

public class UserRiskResponseDto
{
    public required Guid CustomerId { get; init; }
    public required string RiskStatuses { get; init; }
    public required DateTime UpdatedAt { get; init; }
}