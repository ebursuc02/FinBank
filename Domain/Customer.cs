namespace Domain;

public class Customer
{
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Address { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}