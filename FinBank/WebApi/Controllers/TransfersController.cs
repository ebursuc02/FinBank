using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Mvc;

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

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Created();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTransfers(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetTransfersQuery, Result<IEnumerable<TransferDto>>>(
            new GetTransfersQuery{CustomerId = customerId, Iban = accountIban}, ct);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Value);
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

        if (!result.IsSuccess)
             return BadRequest(result.Errors);

        return Ok(result.Value);
    }

}