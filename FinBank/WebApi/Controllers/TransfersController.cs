using Application.DTOs;
using Application.UseCases.Commands.TransferCommands;
using Application.UseCases.Queries.TransferQueries;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.FailHandeling;
using WebApi.Utils;

namespace WebApi.Controllers;

[Route("api/v1/customers/{customerId:Guid}/accounts/{accountIban}/[controller]")]
[ApiController]
public class TransfersController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = AuthorizationPolicies.OwnerOfUserPolicy)]
    [HttpPost]
    public async Task<IActionResult> CreateTransfer(
        [FromBody] CreateTransferCommand command,
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var draftResult = await mediator
            .SendCommandAsync<CreateTransferCommand, Result<Guid>>(command, ct);
        
        if (draftResult.IsFailed)
            return draftResult.ToErrorResponseOrNull(this) ?? Created();

        var transferId = draftResult.Value;

        var finalResult = await TransferUtils.CompleteOrDenyTransferAsync(mediator, transferId, ct);

        return finalResult.ToErrorResponseOrNull(this) ?? Created();
    }

    [Authorize(Policy = AuthorizationPolicies.OwnerOfUserPolicy)]
    [HttpGet("{transferId:Guid}")]
    public async Task<IActionResult> GetTransferById(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        [FromRoute] Guid transferId,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetTransferByIdQuery, Result<TransferDto>>(
            new GetTransferByIdQuery { CustomerId = customerId, Iban = accountIban, TransferId = transferId }, ct);

        return result.ToErrorResponseOrNull(this) ?? Ok(result.Value);
    }
    
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.OwnerOfUserPolicy)]
    public async Task<IActionResult> GetTransfers(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        [FromQuery] TransferStatus? status,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetTransfersByCustomerIdOrStatusQuery, Result<List<TransferDto>>>(
            new GetTransfersByCustomerIdOrStatusQuery(customerId, accountIban, status), ct);
        
        return  result.ToErrorResponseOrNull(this) ?? Ok(result.Value);
    }
}