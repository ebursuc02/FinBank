using Domain.Enums;

namespace Domain;

public class Transfer
{
    public Guid TransferId { get; set; } // PK
    public string FromIban { get; set; } = string.Empty;
    public string ToIban { get; set; } = string.Empty;
    public Guid? ReviewedBy { get; set; } 
    public DateTime CreatedAt { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    public decimal Amount { get; set; } 
    public string Currency { get; set; } = string.Empty; 
    public string? Reason { get; set; } 
    public DateTime? CompletedAt { get; set; }
    public string? PolicyVersion { get; set; }
    
    public Account? FromAccount { get; set; }
    public Account? ToAccount { get; set; }
    public User? Reviewer { get; set; }
    public ICollection<IdempotencyKey> IdempotencyKeys { get; set; } = new List<IdempotencyKey>();
    
}