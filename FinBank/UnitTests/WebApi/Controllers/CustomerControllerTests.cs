using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces.Security;
using Application.UseCases.Commands.UserCommands;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace UnitTests.WebApi.Controllers;

[TestFixture]
public class CustomerControllerTests
{
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IJwtTokenService> _jwtMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _jwtMock = new Mock<IJwtTokenService>();
    }
    
    private Customer CreateController(ClaimsPrincipal? user = null)
    {
        var controller = new Customer(_mediatorMock.Object, _jwtMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        return controller;
    }

    private static ClaimsPrincipal CreateUserWithSub(Guid id)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString())
        }, "TestAuth");

        return new ClaimsPrincipal(identity);
    }
    

    private async Task<OkObjectResult> ExecuteRegisterSuccessAsync()
    {
        var controller = CreateController();

        var userDto = new UserDto
        {
            UserId = Guid.NewGuid(),
            Email = "test@test.com",
            Role = UserRole.Customer,
            Cnp = "1234567890123",
        };

        var cmd = new RegisterUserCommand { Role = UserRole.Customer };

        _mediatorMock
            .Setup(m => m.SendCommandAsync<RegisterUserCommand, Result<UserDto>>(
                cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userDto));

        _jwtMock
            .Setup(j => j.GenerateToken(userDto))
            .Returns("fake-jwt-token");

        var result = await controller.Register(cmd, CancellationToken.None);
        return (OkObjectResult)result;
    }

    private async Task<OkObjectResult> ExecuteLoginSuccessAsync()
    {
        var controller = CreateController();

        var userDto = new UserDto
        {
            UserId = Guid.NewGuid(),
            Email = "test@test.com",
            Role = UserRole.Customer,
            Cnp = "123456"
        };

        var cmd = new LoginUserCommand("email", "password");

        _mediatorMock
            .Setup(m => m.SendCommandAsync<LoginUserCommand, Result<UserDto>>(
                cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userDto));

        _jwtMock.Setup(j => j.GenerateToken(userDto)).Returns("token123");

        var result = await controller.Login(cmd, CancellationToken.None);
        return (OkObjectResult)result;
    }

    private static (string? Message, string? Token) ExtractMessageAndToken(object body)
    {
        var type = body.GetType();
        var messageProp = type.GetProperty("message");
        var tokenProp   = type.GetProperty("token");

        var message = (string?)messageProp?.GetValue(body);
        var token   = (string?)tokenProp?.GetValue(body);

        return (message, token);
    }

    [Test]
    public async Task Register_InvalidRole_ReturnsBadRequest()
    {
        var controller = CreateController();
        var cmd = new RegisterUserCommand { Role = "InvalidRole" };

        var result = await controller.Register(cmd, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        Assert.That(badRequest!.Value, Is.EqualTo("Invalid or unsupported role."));
    }
    

    [Test]
    public async Task Register_Success_ReturnsOkObjectResult()
    {
        var ok = await ExecuteRegisterSuccessAsync();
        Assert.That(ok, Is.Not.Null);
    }

    [Test]
    public async Task Register_Success_ReturnsCorrectMessage()
    {
        var ok = await ExecuteRegisterSuccessAsync();
        var (message, _) = ExtractMessageAndToken(ok.Value!);

        Assert.That(message, Is.EqualTo("User registered successfully"));
    }

    [Test]
    public async Task Register_Success_ReturnsCorrectToken()
    {
        var ok = await ExecuteRegisterSuccessAsync();
        var (_, token) = ExtractMessageAndToken(ok.Value!);

        Assert.That(token, Is.EqualTo("fake-jwt-token"));
    }

    [Test]
    public async Task Login_Success_ReturnsOkObjectResult()
    {
        var ok = await ExecuteLoginSuccessAsync();
        Assert.That(ok, Is.Not.Null);
    }

    [Test]
    public async Task Login_Success_ReturnsCorrectMessage()
    {
        var ok = await ExecuteLoginSuccessAsync();
        var (message, _) = ExtractMessageAndToken(ok.Value!);

        Assert.That(message, Is.EqualTo("User logged in successfully"));
    }

    [Test]
    public async Task Login_Success_ReturnsCorrectToken()
    {
        var ok = await ExecuteLoginSuccessAsync();
        var (_, token) = ExtractMessageAndToken(ok.Value!);

        Assert.That(token, Is.EqualTo("token123"));
    }


}
