using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.UseCases.Queries.CustomerQueries;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("v1/costumers")]
public class Customer(IMediator mediator) : ControllerBase
{
    [HttpPost("register", Name = "RegisterUser")]
    public async Task<ActionResult<User>> RegisterUser(CancellationToken ct)
    {
        return BadRequest("Not implemented");
    }
    
    [HttpGet]
    
    [Authorize]
    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetUserById([FromBody]Guid userId, CancellationToken ct)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if(string.IsNullOrEmpty(sub) || Guid.TryParse(sub, out var tokenUserId))
        {
            return Unauthorized();
        }

        if (tokenUserId == userId)
        {
            return Forbid();
        }
        
        var result = await mediator.SendQueryAsync<GetUserByIdQuery, User?>(
            new GetUserByIdQuery(userId), ct);

        return result is null ? NotFound() : Ok(result);
    }
}