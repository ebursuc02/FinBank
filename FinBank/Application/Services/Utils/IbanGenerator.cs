using Application.Interfaces.Utils;
using Domain;
using Microsoft.Extensions.Options;

namespace Application.Services.Utils;

public class IbanGenerator(IOptions<BankConfig> bankConfig) : IIbanGenerator
{
    private readonly string _countryCode = bankConfig.Value.CountryCode;

    public string Generate(Guid customerId)
    {
        var sequence = DateTime.UtcNow.Ticks;
        return $"{_countryCode}{sequence}{customerId.ToString().Substring(0, 6)}";
    }
}
