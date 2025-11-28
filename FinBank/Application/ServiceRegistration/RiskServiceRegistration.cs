using Application.Services.Policies;
using Application.Interfaces.Kyc;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ServiceRegistration
{
    internal static class RiskServiceRegistration
    {
        public static IServiceCollection AddRiskServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IRiskPolicyEvaluator, StatusRiskPolicyEvaluator>()
                .AddScoped<IRiskContext, RiskContext>();
        }
    }
}
