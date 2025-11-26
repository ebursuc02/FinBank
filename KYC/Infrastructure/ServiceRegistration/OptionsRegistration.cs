using Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ServiceRegistration;

internal static class OptionsRegistration
{
    public static IServiceCollection AddInfrastructureOptions(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.AddOptions<ConnectionOptions>()
            .Bind(cfg.GetSection("ConnectionStrings"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.KYC),
                "ConnectionStrings:KYC is required")
            .ValidateOnStart();

        services.AddOptions<PersistenceOptions>()
            .Bind(cfg.GetSection("Ef"))
            .Validate(o => o.CommandTimeoutSeconds > 0, "Ef:CommandTimeoutSeconds must be > 0")
            .Validate(o => !o.EnableRetryOnFailure || o.MaxRetryCount >= 0, "Ef:MaxRetryCount must be >= 0")
            .Validate(o => !o.EnableRetryOnFailure || o.MaxRetryDelaySeconds >= 0, "Ef:MaxRetryDelaySeconds must be >= 0")
            .ValidateOnStart();

        return services;
    }
}