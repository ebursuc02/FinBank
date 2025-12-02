using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.UnitOfWork;
using Application.ValidationPipeline;
using FluentResults;
using Mediator.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace UnitTests.Application.ValidationPipeline;

[TestFixture]
public class TransactionBehaviorTests
{
    private IUnitOfWork _uow;
    private IUnitOfWorkTransaction _transaction;
    private TransactionBehavior<TestCommand, Result> _behavior;
    private Func<Task<Result>> _next;

    public class TestCommand
    {
    }

    public class TestQuery<T> : IQuery<T>
    {
    }

    [SetUp]
    public void SetUp()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _transaction = Substitute.For<IUnitOfWorkTransaction>();
        _uow.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(_transaction);
        _behavior = new TransactionBehavior<TestCommand, Result>(_uow);
        _next = Substitute.For<Func<Task<Result>>>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _transaction.DisposeAsync();
    }

    [Test]
    public async Task Should_NotStartTransaction_ForQuery()
    {
        // Arrange
        var behavior = new TransactionBehavior<TestQuery<int>, Result>(_uow);
        _next.Invoke().Returns(Task.FromResult(Result.Ok()));
        var query = new TestQuery<int>();

        // Act
        var result = await behavior.HandleAsync(query, _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _uow.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_CommitAndSave_ForSuccessfulCommand()
    {
        // Arrange
        _next.Invoke().Returns(Result.Ok());
        var cmd = new TestCommand();

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);
        var beginTransactionCalled = _uow.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        var saveChangesCalled = _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        var commitCalled = _transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        var rollbackNotCalled = _transaction.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            Assert.That(beginTransactionCalled, Is.Not.Null);
            Assert.That(saveChangesCalled, Is.Not.Null);
            Assert.That(commitCalled, Is.Not.Null);
            Assert.That(rollbackNotCalled, Is.Not.Null);
        });
    }

    [Test]
    public async Task Should_Rollback_ForFailedCommand()
    {
        // Arrange
        _next.Invoke().Returns(Result.Fail("fail"));
        var cmd = new TestCommand();

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);
        var beginTransactionCalled = _uow.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        var rollbackCalled = _transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        var commitNotCalled = _transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        var saveChangesNotCalled = _uow.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(beginTransactionCalled, Is.Not.Null);
            Assert.That(rollbackCalled, Is.Not.Null);
            Assert.That(commitNotCalled, Is.Not.Null);
            Assert.That(saveChangesNotCalled, Is.Not.Null);
        });
    }

    [Test]
    public async Task Should_RollbackAndReturnError_OnDbException()
    {
        // Arrange
        _next.Invoke().Throws(new TestDbException("db error"));
        var cmd = new TestCommand();

        // Act
        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);
        var rollbackCalled = _transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors[0].Message, Does.Contain("Unexpected error during transaction"));
            Assert.That(rollbackCalled, Is.Not.Null);
        });
    }

    public class TestDbException(string message) : DbException(message);
}