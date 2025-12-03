using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using Domain;
using FluentResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace UnitTests.Application.UseCases.CommandHandlers
{
    [TestFixture]
    public class ReOpenAccountCommandHandlerTests
    {
        private IAccountRepository _accountRepository;
        private ReOpenAccountCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _accountRepository = Substitute.For<IAccountRepository>();
            _handler = new ReOpenAccountCommandHandler(_accountRepository);
        }

        [Test]
        public async Task HandleAsync_ShouldReOpenAccount_ForValidRequest()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var account = new Account { CustomerId = customerId, IsClosed = true };
            var command = new ReOpenAccountCommand { Iban = "iban1", CustomerId = customerId };
            _accountRepository.GetByIbanAsync("iban1", Arg.Any<CancellationToken>()).Returns(account);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(!account.IsClosed);
                _accountRepository.Received(1).GetByIbanAsync("iban1", Arg.Any<CancellationToken>());
                _accountRepository.Received(1).UpdateAsync(account, Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnConflict_WhenAccountAlreadyOpen()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var account = new Account { CustomerId = customerId, IsClosed = false };
            var command = new ReOpenAccountCommand { Iban = "iban3", CustomerId = customerId };
            _accountRepository.GetByIbanAsync("iban3", Arg.Any<CancellationToken>()).Returns(account);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors[0], Is.TypeOf<ConflictError>());
                _accountRepository.Received(1).GetByIbanAsync("iban3", Arg.Any<CancellationToken>());
                _accountRepository.DidNotReceive().UpdateAsync(account, Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public void HandleAsync_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new ReOpenAccountCommand { Iban = "iban4", CustomerId = customerId };
            _accountRepository.GetByIbanAsync("iban4", Arg.Any<CancellationToken>()).Throws(new System.Exception("db error"));

            // Act & Assert
            Assert.ThrowsAsync<System.Exception>(async () =>
                await _handler.HandleAsync(command, CancellationToken.None));
        }
    }
}

