using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;

namespace WebApi.Controllers;

[Route("api/v1/transfers-approval")]
[ApiController]
public class TransfersApprovalController(IMediator mediator):ControllerBase
{
    [Authorize]
    [HttpGet("transfers")]
    public async Task<ActionResult<Result<IList<TransferDto>>>> GetTransfersByStatus([FromQuery] TransferStatus status,
        CancellationToken ct)
    {
        
        var role = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (string.IsNullOrEmpty(role))
        {
            return Unauthorized();
        }
        
        var result = await mediator.SendCommandAsync<GetTransferApprovalByStatusCommand, Result<List<TransferDto>>>(
            new GetTransferApprovalByStatusCommand(status), ct);

        if (result.IsFailed)
        {
            return NotFound("No transactions found.");
        }

        var approvalList = result.Value;

        return Ok(approvalList);
    }
}