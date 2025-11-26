using Domain;
using Domain.Enums;

namespace Application.DTOs;

public enum TransferDirection { Ingoing, Outgoing }

public class TransferOverviewDto
{
    public Guid TransferId { get; init; }
    public string TransferDirectionType { get; init; } = string.Empty;
    public string DisplayedName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public TransferStatus Status { get; set; }
    public decimal Amount { get; set; } 
    public string Currency { get; set; } = string.Empty; 
}