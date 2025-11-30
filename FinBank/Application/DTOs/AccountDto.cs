using Domain;
using Domain.Enums;

namespace Application.DTOs;

public class AccountDto
{
    public string Iban { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal Balance { get; init; }
    public string Currency { get; init; }
}