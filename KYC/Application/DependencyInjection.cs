using Application.DTOs;
using Application.Mapping;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator.Mediator>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddScoped<IQueryHandler<GetRiskStatusCommand, IResult<UserRiskDto>>, GetRiskStatusCommandHandler>();
        
        services.AddAutoMapper(_ => { }, typeof(ApplicationMappingProfile).Assembly);
        return services;
    }
}