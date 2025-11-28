namespace WebApi.DTOs.Response;

public class AccountResponseDto
{
    public string Iban { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal Balance { get; init; } 
    public string Currency { get; init; }
}