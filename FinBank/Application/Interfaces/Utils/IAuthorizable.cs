namespace Application.Interfaces.Utils;

public interface IAuthorizable
{
    public Guid CustomerId { get; init; }
    public string Iban { get; init; }
}