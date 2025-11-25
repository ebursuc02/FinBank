using Application.UseCases.Commands;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/v1/customers/{customerId:Guid}/[controller]")]
[ApiController]
public class TransfersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTransfer(
        [FromBody] CreateTransferCommand command, 
        Guid customerId,
        CancellationToken ct)
    {
        if (!ModelState.IsValid || customerId != command.CustomerId )
        {
            return BadRequest(ModelState);
        }

        var result = await mediator.SendCommandAsync<CreateTransferCommand, Result>(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Created();
    }

}