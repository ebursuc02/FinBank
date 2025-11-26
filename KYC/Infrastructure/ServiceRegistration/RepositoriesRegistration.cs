using Application.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ServiceRegistration;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRiskRepository, UserRiskRepository>();

        return services;
    }
}