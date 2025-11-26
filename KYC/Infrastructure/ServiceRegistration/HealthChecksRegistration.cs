using HealthChecks.SqlServer;
using Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Infrastructure.ServiceRegistration;

internal static class HealthChecksRegistration
{
    public static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().Add(new HealthCheckRegistration(
            name: "KYCDB",
            factory: sp =>
            {
                var conn = sp.GetRequiredService<IOptions<ConnectionOptions>>().Value.KYC;
                return new SqlServerHealthCheck(new SqlServerHealthCheckOptions { ConnectionString = conn });
            },
            failureStatus: HealthStatus.Degraded,
            tags: []));

        return services;
    }
}