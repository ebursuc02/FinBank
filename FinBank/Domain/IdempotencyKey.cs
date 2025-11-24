namespace Domain;

public class IdempotencyKey
{
    public string IdempotencyKeyValue { get; set; } = string.Empty;
    public Guid TransferId { get; set; }
    public string? RequestHash { get; set; }
    public string? ResponseJson { get; set; }
    public DateTime? FirstProcessedAt { get; set; }

    public Transfer? Transfer { get; set; }
}