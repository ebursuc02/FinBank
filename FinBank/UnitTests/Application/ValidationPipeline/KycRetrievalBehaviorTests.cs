using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Application.UseCases.ValidationPipeline;
using Domain.Enums;
using Domain.Kyc;
using Domain.Policies;
using FluentResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace UnitTests.Application.ValidationPipeline;

[TestFixture]
public class KycRetrievalBehaviorTests
{
    private IRiskClient _riskClient;
    private IRiskContext _riskContext;
    private IUserRepository _userRepository;
    private KycRetrievalBehavior<CreateTransferCommand, Result> _behavior;
    private Func<Task<Result>> _next;

    [SetUp]
    public void SetUp()
    {
        _riskClient = Substitute.For<IRiskClient>();
        _riskContext = Substitute.For<IRiskContext>();
        _userRepository = Substitute.For<IUserRepository>();
        _behavior = new KycRetrievalBehavior<CreateTransferCommand, Result>(_riskClient, _userRepository, _riskContext);
        _next = Substitute.For<Func<Task<Result>>>();
    }

    [Test]
    public async Task Should_Skip_ForNonCreateTransferCommand()
    {
        // Arrange
        var behavior = new KycRetrievalBehavior<object, Result>(_riskClient, _userRepository, _riskContext);
        _next.Invoke().Returns(Result.Ok());
        var request = new object();

        // Act
        var result = await behavior.HandleAsync(request, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _riskClient.DidNotReceiveWithAnyArgs().GetAsync(default, default);
            _riskContext.DidNotReceive().Current = Arg.Any<RiskStatus>();
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_SetRiskContext_ToReturnedRisk_OnSuccessfulFetch()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cnp = "1234567890123";
        var cmd = new CreateTransferCommand
        {
            CustomerId = customerId,
            PolicyVersion = "v2",
            Iban = "iban1",
            ToIban = "iban2",
            Amount = 100,
            Currency = "EUR"
        };
        _userRepository.GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(cnp);
        _riskClient.GetAsync(cnp, Arg.Any<CancellationToken>())
            .Returns(Result.Ok(RiskStatus.High));

        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _userRepository.Received(1).GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>());
            _riskClient.Received(1).GetAsync(cnp, Arg.Any<CancellationToken>());
            _riskContext.Received(1).Current = RiskStatus.High;
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_SetRiskContext_ToMedium_WhenRiskFetchFails()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cnp = "1234567890123";
        var cmd = new CreateTransferCommand
        {
            CustomerId = customerId,
            PolicyVersion = null,
            Iban = "iban1",
            ToIban = "iban2",
            Amount = 100,
            Currency = "EUR"
        };
        _userRepository.GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(cnp);
        _riskClient.GetAsync(cnp, Arg.Any<CancellationToken>()).Returns(Result.Fail("fail"));
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _userRepository.Received(1).GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>());
            _riskClient.Received(1).GetAsync(cnp, Arg.Any<CancellationToken>());
            _riskContext.Received(1).Current = RiskStatus.Medium;
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_SetMediumRisk_IfUserNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cmd = new CreateTransferCommand
        {
            CustomerId = customerId,
            PolicyVersion = null,
            Iban = "iban1",
            ToIban = "iban2",
            Amount = 100,
            Currency = "EUR"
        };
        _userRepository.GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((string?)null);
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _userRepository.Received(1).GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>());
            _riskClient.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            _riskContext.Received(1).Current = RiskStatus.Medium;
            _next.Received(1)();
        });
    }

    [Test]
    public void Should_PropagateExceptions_FromRiskClient()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cnp = "1234567890123";
        var cmd = new CreateTransferCommand
        {
            CustomerId = customerId,
            PolicyVersion = "v1",
            Iban = "iban1",
            ToIban = "iban2",
            Amount = 100,
            Currency = "EUR"
        };
        _userRepository.GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(cnp);
        _riskClient.GetAsync(cnp, Arg.Any<CancellationToken>()).Throws(new Exception("kyc error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () =>
            await _behavior.HandleAsync(cmd, _next, CancellationToken.None));
    }
}