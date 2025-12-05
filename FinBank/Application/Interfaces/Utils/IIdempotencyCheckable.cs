namespace Application.Interfaces.Utils;

public interface IIdempotencyCheckable
{
    public Guid IdempotencyKey { get; }
}