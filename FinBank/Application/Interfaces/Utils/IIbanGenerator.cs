namespace Application.Interfaces.Utils;

public interface IIbanGenerator
{
    string Generate(Guid customerId);
}
