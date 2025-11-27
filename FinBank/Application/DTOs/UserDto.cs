namespace Application.DTOs;

public class UserDto
{
    public required string Email { get; init; }
    public string Role { get; set; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Country { get; init; }
    public DateTime? Birthday { get; init; }
    public string? Address { get; init; }
}