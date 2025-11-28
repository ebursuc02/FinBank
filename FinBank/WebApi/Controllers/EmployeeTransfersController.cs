using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/v1/transfers")]
[ApiController]
// [Authorize(Roles = "Employee")]
public class EmployeeTransfersController(IMediator mediator) : ControllerBase
{
    [HttpGet("transfers")]
    public async Task<ActionResult<Result<IList<TransferDto>>>> GetTransfersByStatus([FromQuery] TransferStatus? status,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetTransferApprovalByStatusQuery, Result<List<TransferDto>>>(
            new GetTransferApprovalByStatusQuery(status), ct);

        if (result.IsFailed)
        {
            return NotFound("No transactions found.");
        }

        var approvalList = result.Value;

        return Ok(approvalList);
    }

    [HttpPatch("{transferId:guid}/accept")]
    public async Task<IActionResult> AcceptTransaction(
        Guid transferId, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<AcceptTransferCommand, Result>(
            new AcceptTransferCommand(transferId), ct);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }
        
        return NoContent();
    }

}