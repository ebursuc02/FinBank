namespace Domain;

public class User
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Address { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}