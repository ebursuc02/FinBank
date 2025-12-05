using Application.DTOs;
using Application.UseCases.Commands;
using AutoMapper;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.FailHandeling;

namespace WebApi.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class KycController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Route("{userCnp}")]
    public async Task<IActionResult> GetUserKycAsync(string userCnp, CancellationToken cancellationToken)
    {
        var command = new GetRiskStatusCommand { UserCnp = userCnp };
        var result =
            await mediator.SendQueryAsync<GetRiskStatusCommand, Result<UserRiskDto>>(command, cancellationToken);

        return result.ToErrorResponseOrNull(this) ?? Ok(mapper.Map<UserRiskResponseDto>(result.Value));
    }
}