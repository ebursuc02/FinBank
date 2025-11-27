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
using WebApi.FailHandeling;

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

        return result.ToErrorResponseOrNull(this) ?? Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts(
        [FromRoute] Guid customerId,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetAllAccountsQuery, Result<IEnumerable<AccountDto>>>(
            new GetAllAccountsQuery { CustomerId = customerId }, ct);

        return result.ToErrorResponseOrNull(this) ??
               Ok(result.Value.Select(mapper.Map<AccountResponseDto>));
    }

    [HttpGet]
    [Route("{accountIban}")]
    public async Task<IActionResult> GetAccount(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var result = await mediator.SendQueryAsync<GetAccountQuery, Result<AccountDto>>(
            new GetAccountQuery { AccountIban = accountIban }, ct);

        return result.ToErrorResponseOrNull(this) ?? Ok(mapper.Map<AccountResponseDto>(result.Value));
    }

    [HttpDelete]
    [Route("{accountIban}")]
    public async Task<IActionResult> CloseAccount(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var command = new CloseAccountCommand { CustomerId = customerId, AccountIban = accountIban };
        var result = await mediator.SendCommandAsync<CloseAccountCommand, Result>(command, ct);

        return result.ToErrorResponseOrNull(this) ?? NoContent();
    }

    [HttpPut]
    [Route("{accountIban}/reopen")]
    public async Task<IActionResult> ReopenAccount(
        [FromRoute] Guid customerId,
        [FromRoute] string accountIban,
        CancellationToken ct)
    {
        var command = new ReOpenAccountCommand { CustomerId = customerId, AccountIban = accountIban };
        var result = await mediator.SendCommandAsync<ReOpenAccountCommand, Result>(command, ct);

        return result.ToErrorResponseOrNull(this) ?? NoContent();
    }
}