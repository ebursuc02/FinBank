using Application.Mapping;
using Application.ServiceRegistration;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddScoped<IMediator, Mediator.Mediator>()
            .AddPipelineBehaviors()
            .AddUserCommands()
            .AddAccountCommands()
            .AddTransferCommands()
            .AddRiskServices()
            .AddUtilityServices();

        services.AddAutoMapper(_ => { }, typeof(ApplicationMappingProfile).Assembly);
        return services;
    }
}