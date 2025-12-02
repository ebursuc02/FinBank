using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces.Security;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.Queries.CustomerQueries;
using Microsoft.AspNetCore.Mvc;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Authorization;
using WebApi.FailHandeling;


namespace WebApi.Controllers;

[ApiController]
[Route("v1/customers")]
public class Customer(IMediator mediator, IJwtTokenService jwt) : ControllerBase
{
    [HttpPost("register", Name = "Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<RegisterUserCommand, Result<UserDto>>(command, ct);

        var possibleError = result.ToErrorResponseOrNull(this);
        if (possibleError is not null) return possibleError;

        var token = jwt.GenerateToken(result.Value);
        return Ok(new { message = "User registered successfully", token });
    }

    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<LoginUserCommand, Result<UserDto>>(command, ct);

        var possibleError = result.ToErrorResponseOrNull(this);
            if (possibleError is not null) return possibleError;
        
        var token = jwt.GenerateToken(result.Value);
        return Ok(new { message = "User logged in successfully", token });
    }

    [HttpDelete("delete", Name = "DeleteUser")]
    public async Task<IActionResult> DeleteUser(CancellationToken ct)
    {
        var userIdString =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized("Invalid or missing userId in JWT");

        var result = await mediator.SendCommandAsync<DeleteUserCommand, Result>(
            new DeleteUserCommand(userId), ct);

        return result.ToErrorResponseOrNull(this) ?? Ok("User delete successful");
    }

    [Authorize]
    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid userId, CancellationToken ct)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var tokenUserId))
        {
            return Unauthorized();
        }

        if (tokenUserId != userId)
        {
            return Forbid();
        }

        var result = await mediator.SendQueryAsync<GetUserByIdQuery, Result<UserDto>>(
            new GetUserByIdQuery(userId), ct);

        return result.ToErrorResponseOrNull(this) ?? Ok(result.Value);
    }
}