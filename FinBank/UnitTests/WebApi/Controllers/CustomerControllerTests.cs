using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Security;
using Application.UseCases.Commands.UserCommands;
using Application.UseCases.Queries.CustomerQueries;
using Domain;
using FluentResults;
using Mediator.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
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

    // ---------- REGISTER ----------

    [Test]
    public async Task Register_InvalidRole_ReturnsBadRequest()
    {
        var controller = CreateController();

        // Role is just a string, so use any invalid string
        var cmd = new RegisterUserCommand { Role = "InvalidRole" };

        var result = await controller.Register(cmd, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        Assert.That(badRequest!.Value, Is.EqualTo("Invalid or unsupported role."));
    }

    [Test]
    public async Task Register_ValidRole_Success_ReturnsOkWithToken()
    {
        var controller = CreateController();

        var userDto = new UserDto
        {
            UserId = Guid.NewGuid(),
            Email = "test@test.com",
            Role = UserRole.Customer
        };

        var cmd = new RegisterUserCommand { Role = UserRole.Customer };

        _mediatorMock
            .Setup(m => m.SendCommandAsync<RegisterUserCommand, Result<UserDto>>(cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userDto));

        _jwtMock
            .Setup(j => j.GenerateToken(userDto))
            .Returns("fake-jwt-token");

        var result = await controller.Register(cmd, CancellationToken.None);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);

        var body = ok!.Value!;
        var bodyType = body.GetType();

        var messageProp = bodyType.GetProperty("message");
        var tokenProp = bodyType.GetProperty("token");

        Assert.That(messageProp, Is.Not.Null);
        Assert.That(tokenProp, Is.Not.Null);

        var message = (string?)messageProp!.GetValue(body);
        var token = (string?)tokenProp!.GetValue(body);

        Assert.That(message, Is.EqualTo("User registered successfully"));
        Assert.That(token, Is.EqualTo("fake-jwt-token"));
    }


    [Test]
    public async Task Register_MediatorFailure_ReturnsNonOk()
    {
        var controller = CreateController();
        var cmd = new RegisterUserCommand { Role = UserRole.Customer };

        _mediatorMock
            .Setup(m => m.SendCommandAsync<RegisterUserCommand, Result<UserDto>>(cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<UserDto>("Some error"));

        var result = await controller.Register(cmd, CancellationToken.None);

        Assert.That(result, Is.Not.InstanceOf<OkObjectResult>());
    }

    // ---------- LOGIN ----------

    [Test]
    public async Task Login_Success_ReturnsOkWithToken()
    {
        var controller = CreateController();

        var userDto = new UserDto
        {
            UserId = Guid.NewGuid(),
            Email = "test@test.com",
            Role = UserRole.Customer
        };

        var cmd = new LoginUserCommand("email", "password");

        _mediatorMock
            .Setup(m => m.SendCommandAsync<LoginUserCommand, Result<UserDto>>(cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userDto));

        _jwtMock.Setup(j => j.GenerateToken(userDto)).Returns("token123");

        var result = await controller.Login(cmd, CancellationToken.None);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);

        var body = ok!.Value!;
        var bodyType = body.GetType();

        var messageProp = bodyType.GetProperty("message");
        var tokenProp = bodyType.GetProperty("token");

        Assert.That(messageProp, Is.Not.Null);
        Assert.That(tokenProp, Is.Not.Null);

        var message = (string?)messageProp!.GetValue(body);
        var token = (string?)tokenProp!.GetValue(body);

        Assert.That(message, Is.EqualTo("User logged in successfully"));
        Assert.That(token, Is.EqualTo("token123"));
    }


    [Test]
    public async Task Login_Failure_ReturnsNonOk()
    {
        var controller = CreateController();
        var cmd = new LoginUserCommand("email", "password");

        _mediatorMock
            .Setup(m => m.SendCommandAsync<LoginUserCommand, Result<UserDto>>(cmd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<UserDto>("bad credentials"));

        var result = await controller.Login(cmd, CancellationToken.None);

        Assert.That(result, Is.Not.InstanceOf<OkObjectResult>());
    }

    // ---------- DELETE USER ----------

    [Test]
    public async Task DeleteUser_InvalidClaim_ReturnsUnauthorized()
    {
        var controller = CreateController();

        var result = await controller.DeleteUser(CancellationToken.None);

        var unauthorized = result as UnauthorizedObjectResult;

        Assert.That(unauthorized, Is.Not.Null);
        Assert.That(unauthorized!.Value, Is.EqualTo("Invalid or missing userId in JWT"));
    }

    [Test]
    public async Task DeleteUser_ValidClaim_Success_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var controller = CreateController(CreateUserWithSub(userId));

        _mediatorMock
            .Setup(m => m.SendCommandAsync<DeleteUserCommand, Result>(
                It.Is<DeleteUserCommand>(c => c.UserId == userId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var result = await controller.DeleteUser(CancellationToken.None);

        var ok = result as OkObjectResult;

        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo("User delete successful"));
    }

    [Test]
    public async Task DeleteUser_ValidClaim_Failure_ReturnsNonOk()
    {
        var userId = Guid.NewGuid();
        var controller = CreateController(CreateUserWithSub(userId));

        _mediatorMock
            .Setup(m => m.SendCommandAsync<DeleteUserCommand, Result>(
                It.IsAny<DeleteUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("something went wrong"));

        var result = await controller.DeleteUser(CancellationToken.None);

        Assert.That(result, Is.Not.InstanceOf<OkObjectResult>());
    }

    // ---------- GET USER BY ID ----------

    [Test]
    public async Task GetUserById_InvalidTokenUser_ReturnsUnauthorized()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "not-a-guid")
        }, "TestAuth");

        var controller = CreateController(new ClaimsPrincipal(identity));

        var result = await controller.GetUserById(Guid.NewGuid(), CancellationToken.None);

        Assert.That(result, Is.TypeOf<UnauthorizedResult>());
    }

    [Test]
    public async Task GetUserById_TokenUserDiffers_ReturnsForbid()
    {
        var tokenUserId = Guid.NewGuid();
        var controller = CreateController(CreateUserWithSub(tokenUserId));

        var result = await controller.GetUserById(Guid.NewGuid(), CancellationToken.None);

        Assert.That(result, Is.TypeOf<ForbidResult>());
    }

    [Test]
    public async Task GetUserById_SameUser_Success_ReturnsOkWithUser()
    {
        var userId = Guid.NewGuid();
        var controller = CreateController(CreateUserWithSub(userId));

        var userDto = new UserDto
        {
            UserId = userId,
            Email = "test@test.com",
            Role = UserRole.Customer
        };

        _mediatorMock
            .Setup(m => m.SendQueryAsync<GetUserByIdQuery, Result<UserDto>>(
                It.Is<GetUserByIdQuery>(q => q.UserId == userId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userDto));

        var result = await controller.GetUserById(userId, CancellationToken.None);

        var ok = result as OkObjectResult;

        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.TypeOf<UserDto>());

        var dto = (UserDto)ok.Value!;
        Assert.That(dto.UserId, Is.EqualTo(userId));
    }

    [Test]
    public async Task GetUserById_SameUser_Failure_ReturnsNonOk()
    {
        var userId = Guid.NewGuid();
        var controller = CreateController(CreateUserWithSub(userId));

        _mediatorMock
            .Setup(m => m.SendQueryAsync<GetUserByIdQuery, Result<UserDto>>(
                It.IsAny<GetUserByIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<UserDto>("not found"));

        var result = await controller.GetUserById(userId, CancellationToken.None);

        Assert.That(result, Is.Not.InstanceOf<OkObjectResult>());
    }
}
