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
    public class CloseAccountCommandHandlerTests
    {
        private IAccountRepository _accountRepository;
        private CloseAccountCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _accountRepository = Substitute.For<IAccountRepository>();
            _handler = new CloseAccountCommandHandler(_accountRepository);
        }

        [Test]
        public async Task HandleAsync_ShouldCloseAccount_ForValidRequest()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var account = new Account { CustomerId = customerId, IsClosed = false };
            var command = new CloseAccountCommand { AccountIban = "iban1", CustomerId = customerId };
            _accountRepository.GetByIbanAsync("iban1", Arg.Any<CancellationToken>()).Returns(account);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(account.IsClosed);
                _accountRepository.Received(1).GetByIbanAsync("iban1", Arg.Any<CancellationToken>());
                _accountRepository.Received(1).UpdateAsync(account, Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnNotFound_WhenAccountDoesNotExistOrNotBelongToCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _accountRepository.GetByIbanAsync("iban2", Arg.Any<CancellationToken>()).Returns((Account)null!);
            var command = new CloseAccountCommand { AccountIban = "iban2", CustomerId = customerId };

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors[0], Is.TypeOf<NotFoundError>());
                _accountRepository.Received(1).GetByIbanAsync("iban2", Arg.Any<CancellationToken>());
                _accountRepository.DidNotReceive().UpdateAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnConflict_WhenAccountAlreadyClosed()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var account = new Account { CustomerId = customerId, IsClosed = true };
            var command = new CloseAccountCommand { AccountIban = "iban3", CustomerId = customerId };
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
            var command = new CloseAccountCommand { AccountIban = "iban4", CustomerId = customerId };
            _accountRepository.GetByIbanAsync("iban4", Arg.Any<CancellationToken>()).Throws(new Exception("db error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _handler.HandleAsync(command, CancellationToken.None));
        }
    }
}