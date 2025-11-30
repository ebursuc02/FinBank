using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.Queries.TransferQueries;
using Domain;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/v1/customers/{customerId:guid}/transfers")]
[ApiController]
[Authorize]
public class CustomerTransactionController(IMediator mediator):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<IList<TransferDto>>>> GetCustomerTransfers(
        Guid customerId,
        [FromQuery] TransferStatus? status,
        CancellationToken ct)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userId is null || role is null || userId != customerId.ToString() && role != "Employee")
            return Forbid();

        var result = await mediator.SendQueryAsync<GetTransfersByCustomerIdOrStatusQuery, Result<List<TransferDto>>>(
            new GetTransfersByCustomerIdOrStatusQuery(customerId, status), ct);

        if (result.IsFailed)
            return NotFound("No transactions found.");

        return Ok(result.Value);
    }

}