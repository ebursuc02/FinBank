using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.ValidationPipeline;
using Domain;
using FluentResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace UnitTests.Application.ValidationPipeline;

[TestFixture]
public class OwnershipBehaviorTests
{
    private IAccountRepository _repo;
    private TransferPreconditionBehavior<TestRequest, Result> _behavior;
    private Func<Task<Result>> _next;

    public class TestRequest : IAuthorizable
    {
        public string Iban { get; init; }
        public Guid CustomerId { get; init; }
    }

    [SetUp]
    public void SetUp()
    {
        _repo = Substitute.For<IAccountRepository>();
        _behavior = new TransferPreconditionBehavior<TestRequest, Result>(_repo);
        _next = Substitute.For<Func<Task<Result>>>();
    }

    [Test]
    public async Task Should_AllowNext_IfRequestIsNotAuthorizable()
    {
        // Arrange
        var behavior = new TransferPreconditionBehavior<object, Result>(_repo);
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
    public async Task Should_AllowNext_IfAccountExistsAndOwnedByCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var req = new TestRequest { Iban = "iban1", CustomerId = customerId };
        _repo.GetByIbanAsync("iban1", Arg.Any<CancellationToken>()).Returns(new Account { CustomerId = customerId });
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(req, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _repo.Received(1).GetByIbanAsync("iban1", Arg.Any<CancellationToken>());
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_Block_IfAccountDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var req = new TestRequest { Iban = "iban2", CustomerId = customerId };
        _repo.GetByIbanAsync("iban2", Arg.Any<CancellationToken>()).Returns((Account)null);

        // Act
        var result = await _behavior.HandleAsync(req, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailed);
            Assert.That(result.Errors[0].Message, Does.Contain("not owned"));
            _repo.Received(1).GetByIbanAsync("iban2", Arg.Any<CancellationToken>());
            _next.DidNotReceive()();
        });
    }

    [Test]
    public async Task Should_Block_IfAccountNotOwnedByCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customerIdOther = Guid.NewGuid();
        var req = new TestRequest { Iban = "iban3", CustomerId = customerId };
        _repo.GetByIbanAsync("iban3", Arg.Any<CancellationToken>()).Returns(new Account { CustomerId = customerIdOther });

        // Act
        var result = await _behavior.HandleAsync(req, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailed);
            Assert.That(result.Errors[0].Message, Does.Contain("not owned"));
            _repo.Received(1).GetByIbanAsync("iban3", Arg.Any<CancellationToken>());
            _next.DidNotReceive()();
        });
    }

    [Test]
    public async Task Should_HandleRepositoryException_AndReturnFailedResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var req = new TestRequest { Iban = "iban4", CustomerId = customerId };
        _repo.GetByIbanAsync("iban4", Arg.Any<CancellationToken>()).Throws(new Exception("db error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () =>
            await _behavior.HandleAsync(req, _next, CancellationToken.None));
    }
}

