using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Security.Interfaces;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.Queries.CustomerQueries;
using Microsoft.AspNetCore.Mvc;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using WebApi.Models;


namespace WebApi.Controllers;

[ApiController]
[Route("v1/customers")]
public class Customer(IMediator mediator, IJwtTokenService jwt) : ControllerBase
{
    [HttpPost("register", Name = "Register")]
    public async Task<ActionResult> Register([FromBody]RegisterUserCommand command , CancellationToken ct)
    {
        var user = await mediator.SendCommandAsync<RegisterUserCommand, Result<User>>(command, ct);
        switch(user.IsSuccess)
        {
            case true:
                jwt.GenerateToken(user.Value);
                return Ok("User registered successfully");
            
            case false:
                return BadRequest("User registration failed");
        }
        
    }

    [HttpPost("login", Name = "Login")]
    public async Task<ActionResult> Login([FromBody]LoginUserCommand command, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<LoginUserCommand, Result<User>>(command, ct);
    
        if (result.IsFailed)
            return Unauthorized("Inavalid email or password");
    
        var user = result.Value;
        
        var token = jwt.GenerateToken(user);
        
        return Ok(new
        {
            message = "User logged in successfully",
            token
        });
    }
    
    // [Authorize]
    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetUserById([FromRoute] Guid userId, CancellationToken ct)
    {
        // var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        //           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //
        // if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var tokenUserId))
        // {
        //     return Unauthorized();
        // }
        //
        // if (tokenUserId != userId)
        // {
        //     return Forbid();
        // }
        
        var result = await mediator.SendQueryAsync<GetUserByIdQuery, User?>(
            new GetUserByIdQuery(userId), ct);
        
        return result is null ? NotFound() : Ok(result);
    }
}