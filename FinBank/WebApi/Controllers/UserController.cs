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
    public async Task<ActionResult> Register([FromBody]RegisterUserCommand command , CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<RegisterUserCommand, Result<UserDto>>(command, ct);

        if (!result.IsSuccess) return BadRequest("User registration failed");

        var user = new User
        {
            UserId = result.Value.UserId,
            Email = result.Value.Email,
            Name = result.Value.Name,
            PhoneNumber = result.Value.PhoneNumber,
            Country = result.Value.Country,
            Birthday = result.Value.Birthday,
            Address = result.Value.Address,
            Password = result.Value.Password
        };
        
        jwt.GenerateToken(user);
        return Ok(new
        {
            message = "User registered successfully",
            token = jwt.GenerateToken(user)
        });

    }

    [HttpPost("login", Name = "Login")]
    public async Task<ActionResult> Login([FromBody]LoginUserCommand command, CancellationToken ct)
    {
        var result = await mediator.SendCommandAsync<LoginUserCommand, Result<UserDto>>(command, ct);
    
        if (result.IsFailed)
            return Unauthorized("Inavalid email or password");
    
        var user = new User
        {
            UserId = result.Value.UserId,
            Email = result.Value.Email,
            Name = result.Value.Name,
            PhoneNumber = result.Value.PhoneNumber,
            Country = result.Value.Country,
            Birthday = result.Value.Birthday,
            Address = result.Value.Address,
            Password = result.Value.Password
        };
        
        var token = jwt.GenerateToken(user);
        
        return Ok(new
        {
            message = "User logged in successfully",
            token
        });
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
    public async Task<ActionResult<User>> GetUserById([FromRoute] Guid userId, CancellationToken ct)
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
        
        var result = await mediator.SendQueryAsync<GetUserByIdQuery, User?>(
            new GetUserByIdQuery(userId), ct);
        
        return result is null ? NotFound() : Ok(result);
    }
}