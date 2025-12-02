using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.CommandHandlers.TransferCommandHandlers;
using Application.UseCases.Commands.TransferCommands;
using Application.UseCases.Queries;
using Application.UseCases.Queries.TransferQueries;
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
                .AddScoped<ICommandHandler<AcceptTransferCommand, Result>, AcceptTransferCommandHandler>()
                .AddScoped<ICommandHandler<DenyTransferCommand, Result>, DenyTransferCommandHandler>()
                .AddScoped<IQueryHandler<GetTransfersQuery, Result<IEnumerable<TransferDto>>>, GetTransfersQueryHandler>()
                .AddScoped<IQueryHandler<GetTransferByIdQuery, Result<TransferDto>>, GetTransferByIdQueryHandler>()
                .AddScoped<IQueryHandler<GetTransfersByCustomerIdOrStatusQuery, Result<List<TransferDto>>>, GetTransfersByCustomerIdOrStatusQueryHandler>()
                .AddScoped<IQueryHandler<GetTransfersByStatusQuery, Result<List<TransferDto>>>, GetTransfersByStatusQueryHandler>();
        }
    }
}
