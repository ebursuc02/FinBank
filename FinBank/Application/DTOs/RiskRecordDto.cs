namespace Application.DTOs;

public class RiskRecordDto
{
    public required Guid CustomerId { get; init; }
    public required string RiskStatus { get; init; }
    public required DateTime UpdatedAt { get; init; }
}