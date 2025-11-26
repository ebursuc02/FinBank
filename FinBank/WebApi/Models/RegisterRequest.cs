namespace WebApi.Models;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Country { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string? Address { get; set; } = string.Empty;
}