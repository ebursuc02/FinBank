using Application.Interfaces.UnitOfWork;
using Infrastructure.Options;
using Infrastructure.Persistence;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.ServiceRegistration;

internal static class PersistenceRegistration
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services)
    {
        services.AddDbContextPool<FinBankDbContext>((sp, options) =>
        {
            var conn = sp.GetRequiredService<IOptions<ConnectionOptions>>().Value;
            var ef   = sp.GetRequiredService<IOptions<PersistenceOptions>>().Value;

            options.UseSqlServer(conn.FinBank, sql =>
            {
                sql.CommandTimeout(ef.CommandTimeoutSeconds);
                if (ef.EnableRetryOnFailure)
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: ef.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(ef.MaxRetryDelaySeconds),
                        errorNumbersToAdd: null);
                }
            });

            options.EnableDetailedErrors(ef.UseDetailedErrors);
            options.EnableSensitiveDataLogging(ef.UseSensitiveDataLogging);
        });

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }
}