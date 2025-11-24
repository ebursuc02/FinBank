
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) 
        =>
            services.AddScoped<IMediator, Mediator.Mediator>()
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
}