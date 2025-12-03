namespace Domain;

public class User
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Address { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}