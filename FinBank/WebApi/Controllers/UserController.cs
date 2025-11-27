using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using Application.Security.Interfaces;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.Queries.CustomerQueries;
using Microsoft.AspNetCore.Mvc;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers;

[ApiController]
[Route("v1/customers")]
public class Customer(IMediator mediator, IJwtTokenService jwt) : ControllerBase
{
    [HttpPost("register", Name = "Register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<RegisterUserCommand, Result<UserDto>>(command, ct);

        if (!result.IsSuccess) return BadRequest("User registration failed");

        var token = jwt.GenerateToken(result.Value);
        return Ok(new { message = "User registered successfully", token });
    }

    [HttpPost("login", Name = "Login")]
    public async Task<ActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<LoginUserCommand, Result<UserDto>>(command, ct);

        if (result.IsFailed)
            return Unauthorized("Invalid email or password");
        
        var token = jwt.GenerateToken(result.Value);
        return Ok(new { message = "User logged in successfully", token });
    }
    
    [HttpDelete("delete", Name = "DeleteUser")]
    public async Task<ActionResult> DeleteUser(CancellationToken ct)
    {
        var userIdString =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized("Invalid or missing userId in JWT");

        var result = await mediator.SendCommandAsync<DeleteUserCommand, Result>(
            new DeleteUserCommand(userId), ct);

        if (result.IsFailed)
            return BadRequest("Failed to delete user");

        return Ok("User delete successful");
    }

    [Authorize]
    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetUserById([FromRoute] Guid userId, CancellationToken ct)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var role = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var tokenUserId))
        {
            return Unauthorized();
        }

        if (tokenUserId != userId && role != "Employee")
        {
            userId = tokenUserId;
        }

        var result = await mediator.SendQueryAsync<GetUserByIdQuery, Result<UserDto>>(
            new GetUserByIdQuery(userId), ct);

        return result.IsFailed ? NotFound() : Ok(result.Value);
    }
}