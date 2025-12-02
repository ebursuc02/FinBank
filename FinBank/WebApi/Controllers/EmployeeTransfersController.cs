using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Application.UseCases.Queries.TransferQueries;
using Domain;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.FailHandeling;

namespace WebApi.Controllers;

[Route("api/v1/transfers")]
[ApiController]
[Authorize(Roles = UserRole.Banker)] 
public class EmployeeTransfersController(IMediator mediator) : ControllerBase
{
    [HttpGet("transfers")]
    public async Task<ActionResult<Result<IList<TransferDto>>>> GetTransfersByStatus([FromQuery] TransferStatus? status,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetTransfersByStatusQuery, Result<List<TransferDto>>>(
            new GetTransfersByStatusQuery(status), ct);

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

        var errorResponse = result.ToErrorResponseOrNull(this);
        return errorResponse ?? NoContent();
    }
    
    [HttpPatch("{transferId:guid}/deny")]
    public async Task<IActionResult> DenyTransaction(
        Guid transferId, [FromBody] string? reason, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<DenyTransferCommand, Result>(
            new DenyTransferCommand(transferId, reason), ct);

        var errorResponse = result.ToErrorResponseOrNull(this);
        return errorResponse ?? NoContent();
    }

}