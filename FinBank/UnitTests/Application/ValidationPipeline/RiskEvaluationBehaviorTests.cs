using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Application.ValidationPipeline;
using Domain.Enums;
using FluentResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace UnitTests.Application.ValidationPipeline;

[TestFixture]
public class RiskEvaluationBehaviorTests
{
    private IRiskClient _riskClient;
    private IRiskPolicyEvaluator _evaluator;
    private IRiskContext _riskContext;
    private IUserRepository _userRepository;
    private RiskEvaluationBehavior<CreateTransferCommand, Result> _behavior;
    private Func<Task<Result>> _next;

    [SetUp]
    public void SetUp()
    {
        _riskClient = Substitute.For<IRiskClient>();
        _evaluator = Substitute.For<IRiskPolicyEvaluator>();
        _riskContext = Substitute.For<IRiskContext>();
        _userRepository = Substitute.For<IUserRepository>();
        _behavior = new RiskEvaluationBehavior<CreateTransferCommand, Result>(_riskClient, _evaluator, _userRepository, _riskContext);
        _next = Substitute.For<Func<Task<Result>>>();
    }

    [Test]
    public async Task Should_Skip_ForNonCreateTransferCommand()
    {
        // Arrange
        var behavior = new RiskEvaluationBehavior<object, Result>(_riskClient, _evaluator, _userRepository, _riskContext);
        _next.Invoke().Returns(Result.Ok());
        var request = new object();

        // Act
        var result = await behavior.HandleAsync(request, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_SetRiskContext_ForSuccessfulRiskFetch()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cnp = "1234567890123";
        var cmd = new CreateTransferCommand {
            CustomerId = customerId,
            PolicyVersion = "v2",
            Iban = "iban1",
            ToIban = "iban2",
            Amount = 100,
            Currency = "EUR"
        };
        _userRepository.GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(cnp);
        _riskClient.GetAsync(cnp, Arg.Any<CancellationToken>()).Returns(Result.Ok(RiskStatus.High));
        _evaluator.Evaluate(RiskStatus.High, out Arg.Any<string>()!).Returns(x => { x[1] = "reason"; return TransferStatus.Rejected; });
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _userRepository.Received(1).GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>());
            _riskClient.Received(1).GetAsync(cnp, Arg.Any<CancellationToken>());
            _evaluator.Received(1).Evaluate(RiskStatus.High, out Arg.Any<string>());
            _riskContext.Received(1).Current = Arg.Is<RiskContextData>(d => d.Decision == TransferStatus.Rejected && d.Reason == "reason" && d.PolicyVersion == "v2");
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_SetMediumRisk_IfRiskFetchFails()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cnp = "1234567890123";
        var cmd = new CreateTransferCommand {
            CustomerId = customerId,
            PolicyVersion = null,
            Iban = "iban1",
            ToIban = "iban2",
            Amount = 100,
            Currency = "EUR"
        };
        _userRepository.GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns(cnp);
        _riskClient.GetAsync(cnp, Arg.Any<CancellationToken>()).Returns(Result.Fail("fail"));
        _evaluator.Evaluate(RiskStatus.Medium, out Arg.Any<string>()).Returns(x => { x[1] = "default"; return TransferStatus.UnderReview; });
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _userRepository.Received(1).GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>());
            _riskClient.Received(1).GetAsync(cnp, Arg.Any<CancellationToken>());
            _evaluator.Received(1).Evaluate(RiskStatus.Medium, out Arg.Any<string>());
            _riskContext.Received(1).Current = Arg.Is<RiskContextData>(d => d.Decision == TransferStatus.UnderReview && d.Reason == "default" && d.PolicyVersion == "v1");
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_SetMediumRisk_IfUserNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cmd = new CreateTransferCommand {
            CustomerId = customerId,
            PolicyVersion = null,
            Iban = "iban1",
            ToIban = "iban2",
            Amount = 100,
            Currency = "EUR"
        };
        _userRepository.GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>()).Returns((string?)null);
        _evaluator.Evaluate(RiskStatus.Medium, out Arg.Any<string>()).Returns(x => { x[1] = "default"; return TransferStatus.UnderReview; });
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _userRepository.Received(1).GetCustomerCnpByIdAsync(customerId, Arg.Any<CancellationToken>());
            _riskClient.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            _evaluator.Received(1).Evaluate(RiskStatus.Medium, out Arg.Any<string>());
            _riskContext.Received(1).Current = Arg.Is<RiskContextData>(d => d.Decision == TransferStatus.UnderReview && d.Reason == "default" && d.PolicyVersion == "v1");
            _next.Received(1)();
        });
    }

    [Test]
    public void Should_PropagateExceptions_FromDependencies()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var cnp = "1234567890123";
        var cmd = new CreateTransferCommand {
            CustomerId = customerId,
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
