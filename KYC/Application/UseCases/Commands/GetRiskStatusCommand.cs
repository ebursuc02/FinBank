using Application.DTOs;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.Commands;

public class GetRiskStatusCommand : IQuery<IResult<UserRiskDto>>
{
    public required Guid UserId { get; init; }

    public GetRiskStatusCommand()
    {
    }
}