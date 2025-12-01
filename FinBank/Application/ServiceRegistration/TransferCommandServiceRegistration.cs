using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.CommandHandlers;
using Application.UseCases.CommandHandlers.TransferCommandHandlers;
using Application.UseCases.Queries;
using Application.UseCases.QueryHandlers;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ServiceRegistration
{
    internal static class TransferCommandServiceRegistration
    {
        public static IServiceCollection AddTransferCommands(this IServiceCollection services)
        {
            return services
                .AddScoped<ICommandHandler<CreateTransferCommand, Result>, CreateTransferCommandHandler>()
                .AddScoped<IQueryHandler<GetTransfersQuery, Result<IEnumerable<TransferDto>>>, GetTransfersQueryHandler>()
                .AddScoped<IQueryHandler<GetTransferByIdQuery, Result<TransferDto>>, GetTransferByIdQueryHandler>();
        }
    }
}
