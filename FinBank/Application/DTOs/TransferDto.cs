using Domain;
using Domain.Enums;

namespace Application.DTOs;

public enum TransferDirection { Ingoing, Outgoing }

public class TransferDto
{
    public Guid TransferId { get; set; }
    public string FromIban { get; set; } = string.Empty;
    public string ToIban { get; set; } = string.Empty; 
    public Guid? ReviewedBy { get; set; } 
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; } 
    public string Currency { get; set; } = string.Empty; 
    public string? Reason { get; set; } 
    public DateTime? CompletedAt { get; set; }
    public string? PolicyVersion { get; set; }
}