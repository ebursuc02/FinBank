namespace Domain;

public class IdempotencyKey
{
    public Guid IdempotencyKeyValue { get; set; }
    public string? RequestHash { get; set; }
    public string? ResponseJson { get; set; }
    public DateTime? FirstProcessedAt { get; set; }
}