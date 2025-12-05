using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public class GetRiskStatusCommandHandler(IUserRiskRepository repository, IMapper mapper)
    : IQueryHandler<GetRiskStatusCommand, Result<UserRiskDto>>
{
    public async Task<Result<UserRiskDto>> HandleAsync(GetRiskStatusCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var riskStatus = await repository.GetByCustomerByCnpAsync(command.UserCnp, cancellationToken);

            if (riskStatus is null)
                return Result.Fail<UserRiskDto>(new NotFoundError("User not found."));

            var dto = mapper.Map<UserRiskDto>(riskStatus);
            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result.Fail<UserRiskDto>(ex.Message);
        }
    }
}