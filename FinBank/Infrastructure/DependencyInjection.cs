using Application.Interfaces.Kyc;
using Infrastructure.Kyc;
using Infrastructure.ServiceRegistration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.AddHttpClient<IRiskClient, RiskHttpClient>(client =>
        {
            client.BaseAddress = new Uri(cfg["Kyc:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        
        services.AddInfrastructureOptions(cfg)
            .AddInfrastructurePersistence()
            .AddInfrastructureRepositories()
            .AddInfrastructureHealthChecks();
        return services;
    }
}