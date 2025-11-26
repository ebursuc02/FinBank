namespace Domain;

public class Account
{
    private static readonly Dictionary<string, decimal> RateFromEur =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["EUR"] = 1.00m,
            ["RON"] = 5.08m,
            ["USD"] = 1.15m
        };
    public string Iban { get; init; } = string.Empty; // PK
    public Guid CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal Balance { get; set; } 
    public string Currency { get; init; } = string.Empty; // CHAR(3)

    public User? Customer { get; init; }

    public void ApplyTransfer(decimal amount, string currency)
    {
        var factor = RateFromEur[Currency] / RateFromEur[currency];
        Balance += decimal.Round(amount * factor, 2, MidpointRounding.ToEven);
    }
}