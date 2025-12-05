namespace WebApi.DTOs;

public class UserRiskResponseDto
{
    public required string Cnp { get; init; }
    public required string RiskStatus { get; init; }
    public required DateTime UpdatedAt { get; init; }
}