namespace Domain;

public class Transfer
{
    public Guid TransferId { get; set; } // PK
    public string FromAccountId { get; set; } = string.Empty;
    public string ToAccountId { get; set; } = string.Empty; 
    public Guid? ReviewedByEmployeeId { get; set; } 
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; } 
    public string Currency { get; set; } = string.Empty; 
    public string? Reason { get; set; } 
    public DateTime? CompletedAt { get; set; }
    public string? PolicyVersion { get; set; }
    
    public Account? FromAccount { get; set; }
    public Account? ToAccount { get; set; }
    public Employee? ReviewedBy { get; set; }
    public ICollection<IdempotencyKey> IdempotencyKeys { get; set; } = new List<IdempotencyKey>();
}