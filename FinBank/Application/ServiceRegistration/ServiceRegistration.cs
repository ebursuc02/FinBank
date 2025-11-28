using Application.Interfaces.Utils;
using Application.Services.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ServiceRegistration
{
    internal static class ServiceRegistration
    {
        public static IServiceCollection AddUtilityServices(this IServiceCollection services)
        {
            return services.AddSingleton<IIbanGenerator, IbanGenerator>();
        }
    }
}
