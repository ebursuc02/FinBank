using Application.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ServiceRegistration;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository,  AccountRepository>();
        services.AddScoped<ITransferRepository, TransferRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
        return services;
    }
}