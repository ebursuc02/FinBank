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
using WebApi.FailHandeling;

namespace WebApi.Controllers;

[Route("api/v1/customers/{customerId:guid}/transfers")]
[ApiController]
[Authorize(Roles = UserRole.Customer)]
public class CustomerTransactionController(IMediator mediator):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCustomerTransfers(
        Guid customerId,
        [FromQuery] TransferStatus? status,
        CancellationToken ct)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userId is null || role is null || (userId != customerId.ToString() && role != UserRole.Banker))
            return Forbid();

        var result = await mediator.SendQueryAsync<GetTransfersByCustomerIdOrStatusQuery, Result<List<TransferDto>>>(
            new GetTransfersByCustomerIdOrStatusQuery(customerId, status), ct);
        
        return  result.ToErrorResponseOrNull(this) ?? Ok(result.Value);
    }

}