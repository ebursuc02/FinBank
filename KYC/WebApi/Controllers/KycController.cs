using Application.DTOs;
using Application.UseCases.Commands;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class KycController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Route("{userId:Guid}")]
    public async Task<IActionResult> GetUserKycAsync(Guid userId, CancellationToken cancellationToken)
    {
        var command = new GetRiskStatusCommand { UserId = userId };
        var result =
            await mediator.SendQueryAsync<GetRiskStatusCommand, IResult<UserRiskDto>>(command, cancellationToken);

        return result.IsFailed ? NotFound() : Ok(mapper.Map<UserRiskResponseDto>(result.Value));
    }
}