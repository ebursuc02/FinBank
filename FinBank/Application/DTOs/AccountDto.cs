using Domain;
using Domain.Enums;

namespace Application.DTOs;

public class AccountDto
{
    public required string Iban { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal Balance { get; init; }
    public required string Currency { get; init; }
}