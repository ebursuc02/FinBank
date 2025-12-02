using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using Application.UseCases.Queries.TransferQueries;
using Domain;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Mvc;
using WebApi.FailHandeling;

namespace WebApi.Controllers;

[Route("api/v1/customers/{customerId:Guid}/accounts/{accountIban}/[controller]")]
[ApiController]
public class TransfersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTransfer(
        [FromBody] CreateTransferCommand command,
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        if (customerId != command.CustomerId || accountIban != command.Iban)
            return BadRequest(ModelState);

        var result = await mediator.SendCommandAsync<CreateTransferCommand, Result>(command, ct);

        return result.ToErrorResponseOrNull(this) ??Created();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTransfers(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetTransfersQuery, Result<IEnumerable<TransferDto>>>(
            new GetTransfersQuery{CustomerId = customerId, Iban = accountIban}, ct);

        return result.ToErrorResponseOrNull(this) ??Ok(result.Value);
    }
    
    [HttpGet("{transferId:Guid}")]
    public async Task<IActionResult> GetTransfers(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        [FromRoute] Guid transferId,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetTransferByIdQuery, Result<TransferDto>>(
            new GetTransferByIdQuery{CustomerId = customerId, Iban = accountIban, TransferId = transferId}, ct);

        return result.ToErrorResponseOrNull(this) ??Ok(result.Value);
    }
    
    [HttpGet("status")]
    public async Task<IActionResult> GetTransfersByStatus(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        [FromQuery] TransferStatus? status,
        CancellationToken ct)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userId is null || role is null || (userId != customerId.ToString() && role != UserRole.Banker))
            return Forbid();

        var result = await mediator.SendQueryAsync<GetTransfersByCustomerIdOrStatusQuery, Result<List<TransferDto>>>(
            new GetTransfersByCustomerIdOrStatusQuery(customerId, accountIban, status), ct);
        
        return  result.ToErrorResponseOrNull(this) ?? Ok(result.Value);
    }

}