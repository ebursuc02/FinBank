namespace Application.DTOs;

public class RiskRecordDto
{
    public required string Cnp { get; init; }
    public required string RiskStatus { get; init; }
    public required DateTime UpdatedAt { get; init; }
}