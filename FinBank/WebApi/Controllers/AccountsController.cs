using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using AutoMapper;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.DTOs.Request;
using WebApi.DTOs.Response;

namespace WebApi.Controllers;

[Route("api/v1/customers/{customerId:Guid}/[controller]")]
[ApiController]
public class AccountsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountRequestDto requestDto,
        [FromRoute] Guid customerId,
        CancellationToken ct)
    {
        var command = new CreateAccountCommand { CustomerId = customerId, Currency = requestDto.Currency };
        var result = await mediator.SendCommandAsync<CreateAccountCommand, Result<AccountDto>>(command, ct);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Created();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAllAccounts(
        [FromRoute] Guid customerId,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetAllAccountsQuery, Result<IEnumerable<AccountDto>>>(
            new GetAllAccountsQuery { CustomerId = customerId }, ct);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Value.Select(mapper.Map<AccountResponseDto>));
    }

    [HttpGet]
    [Route("{accountIban}")]
    public async Task<ActionResult<AccountResponseDto>> GetAccount(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetAccountQuery, Result<AccountDto>>(
            new GetAccountQuery { AccountIban = accountIban }, ct);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(mapper.Map<AccountResponseDto>(result.Value));
    }

    [HttpDelete]
    [Route("{accountIban}")]
    public async Task<IActionResult> DeleteAccount(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var command = new DeleteAccountCommand { CustomerId = customerId, AccountIban = accountIban };
        var result = await mediator.SendCommandAsync<DeleteAccountCommand, Result>(command, ct);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return NoContent();
    }
}