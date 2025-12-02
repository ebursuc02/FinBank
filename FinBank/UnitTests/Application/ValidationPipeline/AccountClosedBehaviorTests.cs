using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.ValidationPipeline;
using Domain;
using FluentResults;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.Application.ValidationPipeline;

[TestFixture]
public class AccountClosedBehaviorTests
{
    private IAccountRepository _repo;
    private AccountClosedBehavior<TestCommand, Result> _behavior;
    private Func<Task<Result>> _next;
    private Account _account;

    public class TestCommand : IAuthorizable
    {
        public Guid CustomerId { get; init; } = Guid.NewGuid();
        public string Iban { get; init; } = "iban";
    }

    [SetUp]
    public void SetUp()
    {
        _repo = Substitute.For<IAccountRepository>();
        _behavior = new AccountClosedBehavior<TestCommand, Result>(_repo);
        _next = Substitute.For<Func<Task<Result>>>();
        _account = new Account { Iban = "iban", IsClosed = false };
    }

    [Test]
    public async Task Handle_ShouldCallNext_IfRequestIsNotAuthorizable()
    {
        var behavior = new AccountClosedBehavior<NonAuthorizableCommand, Result>(_repo);
        var cmd = new NonAuthorizableCommand();
        _next.Invoke().Returns(Result.Ok());

        var result = await behavior.HandleAsync(cmd, _next, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _repo.DidNotReceive().GetByIbanAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Handle_ShouldCallNext_IfAccountNotFound()
    {
        _repo.GetByIbanAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Account)null);
        var cmd = new TestCommand();
        _next.Invoke().Returns(Result.Ok());

        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _repo.Received(1).GetByIbanAsync(cmd.Iban, Arg.Any<CancellationToken>());
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Handle_ShouldReturnFailedResult_IfAccountIsClosed()
    {
        _account.IsClosed = true;
        _repo.GetByIbanAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_account);
        var cmd = new TestCommand();

        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailed);
            Assert.That(result.Errors[0].Message, Is.EqualTo("Account is closed"));
            _repo.Received(1).GetByIbanAsync(cmd.Iban, Arg.Any<CancellationToken>());
            _next.DidNotReceive()();
        });
    }

    [Test]
    public async Task Handle_ShouldCallNext_IfAccountIsOpen()
    {
        _account.IsClosed = false;
        _repo.GetByIbanAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_account);
        var cmd = new TestCommand();
        _next.Invoke().Returns(Result.Ok());

        var result = await _behavior.HandleAsync(cmd, _next, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            _repo.Received(1).GetByIbanAsync(cmd.Iban, Arg.Any<CancellationToken>());
            _next.Received(1)();
        });
    }

    public class NonAuthorizableCommand { }
}
