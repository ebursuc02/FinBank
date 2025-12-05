using Application.DTOs;
using Application.UseCases.Commands.TransferCommands;
using Application.UseCases.Queries.TransferQueries;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.DTOs.Request;
using WebApi.FailHandeling;

namespace WebApi.Controllers;

[Route("api/v1/customers/{customerId:Guid}/accounts/{accountIban}/[controller]")]
[ApiController]
public class TransfersController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = AuthorizationPolicies.OwnerOfUserPolicy)]
    [HttpPost]
    public async Task<IActionResult> CreateTransfer(
        [FromBody] CreateTransferRequestDto request,
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var createCommand = new CreateTransferCommand { 
            CustomerId = customerId,
            Iban = accountIban,
            ToIban = request.ToIban,
            Amount = request.Amount,
            Currency = request.Currency,
            IdempotencyKey = request.IdempotencyKey,
            PolicyVersion = request.PolicyVersion };
        
        var draftResult = await mediator
            .SendCommandAsync<CreateTransferCommand, Result<Guid>>(createCommand, ct);
        
        if (draftResult.IsFailed)
            return draftResult.ToErrorResponseOrNull(this) ?? Created();

        var transferId = draftResult.Value;

        var finalResult = await CompleteOrDenyTransferAsync(transferId, ct);

        return finalResult.ToErrorResponseOrNull(this) ?? Created();
    }

    private async Task<Result> CompleteOrDenyTransferAsync(Guid transferId, CancellationToken ct)
    {
        var completeResult = await mediator
            .SendCommandAsync<CompleteTransferCommand, Result>(
                new CompleteTransferCommand { TransferId = transferId }, ct);

        if (completeResult.IsSuccess)
            return completeResult;

        var denyCommand = new DenyTransferCommand(
            transferId,
            string.Join("; ", completeResult.Errors.Select(e => e.Message)));

        var denyResult = await mediator
            .SendCommandAsync<DenyTransferCommand, Result>(denyCommand, ct);

        return denyResult;
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