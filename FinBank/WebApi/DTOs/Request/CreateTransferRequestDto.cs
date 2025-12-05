namespace WebApi.DTOs.Request;

public class CreateTransferRequestDto
{
    public required string ToIban { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public string? PolicyVersion { get; init; }
}