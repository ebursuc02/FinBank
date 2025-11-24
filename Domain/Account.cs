namespace Domain;

public class Account
{
    public string Iban { get; set; } = string.Empty; // PK
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Currency { get; set; } = string.Empty; // CHAR(3)

    public Customer? Customer { get; set; }
}