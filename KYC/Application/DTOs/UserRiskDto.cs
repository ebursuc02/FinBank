using Application.DTOs.Enums;
using Domain.Enums;

namespace Application.DTOs;

public class UserRiskDto
{
    public required string Cnp { get; init; }
    public required RiskStatusDto RiskStatus { get; init; }
    public required DateTime UpdatedAt { get; init; }
}