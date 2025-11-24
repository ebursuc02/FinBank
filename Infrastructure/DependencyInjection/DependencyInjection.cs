using Mediator.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.AddInfrastructureOptions(cfg)
            .AddInfrastructurePersistence()
            .AddInfrastructureRepositories()
            .AddInfrastructureHealthChecks();
        
        services.AddScoped<IMediator, Mediator.Mediator>(); 
        
        return services;
    }
}