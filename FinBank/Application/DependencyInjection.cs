using Application.DTOs;
using Application.Interfaces.Kyc;
using Application.Mapping;
using Application.Policies;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.Queries;
using Application.UseCases.Queries.CustomerQueries;
using Application.UseCases.QueryHandlers;
using Application.ValidationPipeline;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Identity;
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