using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers;

public class GetRiskStatusCommandHandler(IUserRiskRepository repository, IMapper mapper)
    : IQueryHandler<GetRiskStatusCommand, IResult<UserRiskDto>>
{
    public async Task<IResult<UserRiskDto>> HandleAsync(GetRiskStatusCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var riskStatus = await repository.GetByCustomerAsync(command.UserId, cancellationToken);

            if (riskStatus is null)
                return Result.Fail<UserRiskDto>("User not found.");

            var dto = mapper.Map<UserRiskDto>(riskStatus);
            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result.Fail<UserRiskDto>(ex.Message);
        }
    }
}