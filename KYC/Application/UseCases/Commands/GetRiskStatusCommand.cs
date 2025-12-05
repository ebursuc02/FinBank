using Application.DTOs;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands;

public class GetRiskStatusCommand : IQuery<Result<UserRiskDto>>
{
    public required string UserCnp { get; init; }

    public GetRiskStatusCommand()
    {
    }
}