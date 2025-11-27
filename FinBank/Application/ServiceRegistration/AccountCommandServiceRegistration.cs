using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Queries;
using Application.UseCases.QueryHandlers;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ServiceRegistration
{
    internal static class AccountCommandServiceRegistration
    {
        public static IServiceCollection AddAccountCommands(this IServiceCollection services)
        {
            return services
                .AddScoped<ICommandHandler<CreateAccountCommand, Result<AccountDto>>, CreateAccountCommandHandler>()
                .AddScoped<ICommandHandler<CloseAccountCommand, Result>, CloseAccountCommandHandler>()
                .AddScoped<ICommandHandler<ReOpenAccountCommand, Result>, ReOpenAccountCommandHandler>()
                .AddScoped<IQueryHandler<GetAccountQuery, Result<AccountDto>>, GetAccountQueryHandler>()
                .AddScoped<IQueryHandler<GetAllAccountsQuery, Result<IEnumerable<AccountDto>>>, GetAllAccountsQueryHandler>();
        }
    }
}
