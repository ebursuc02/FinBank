using System;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using AutoMapper;
using Domain;
using FluentResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace UnitTests.Application.UseCases.CommandHandlers
{
    [TestFixture]
    public class CreateAccountCommandHandlerTests
    {
        private IIbanGenerator _ibanGenerator;
        private IUserRepository _userRepository;
        private IAccountRepository _accountRepository;
        private IMapper _mapper;
        private CreateAccountCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _ibanGenerator = Substitute.For<IIbanGenerator>();
            _userRepository = Substitute.For<IUserRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new CreateAccountCommandHandler(_ibanGenerator, _userRepository, _accountRepository, _mapper);
        }

        [Test]
        public async Task HandleAsync_ShouldCreateAccount_ForValidCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var user = new User { UserId = customerId, Role = UserRole.Customer };
            var iban = "IBAN123";
            var account = new Account { Iban = iban, CustomerId = customerId };
            var accountDto = new AccountDto { Iban = iban, CustomerId = customerId, Currency = "EUR"};
            var command = new CreateAccountCommand { CustomerId = customerId, Currency = "EUR", InitialDeposit = 100 };
            _userRepository.GetAsync(customerId, Arg.Any<CancellationToken>()).Returns(user);
            _ibanGenerator.Generate(customerId).Returns(iban);
            _mapper.Map<AccountDto>(Arg.Any<Account>()).Returns(accountDto);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Value, Is.EqualTo(accountDto));
                _userRepository.Received(1).GetAsync(customerId, Arg.Any<CancellationToken>());
                _accountRepository.Received(1).AddAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new CreateAccountCommand { CustomerId =  customerId };
            _userRepository.GetAsync(customerId, Arg.Any<CancellationToken>()).Returns((User)null);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors[0], Is.TypeOf<NotFoundError>());
                _userRepository.Received(1).GetAsync(customerId, Arg.Any<CancellationToken>());
                _accountRepository.DidNotReceive().AddAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnValidationError_WhenUserIsNotCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var user = new User { UserId = customerId, Role = UserRole.Admin };
            var command = new CreateAccountCommand { CustomerId = customerId };
            _userRepository.GetAsync(customerId, Arg.Any<CancellationToken>()).Returns(user);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors[0], Is.TypeOf<ValidationError>());
                _userRepository.Received(1).GetAsync(customerId, Arg.Any<CancellationToken>());
                _accountRepository.DidNotReceive().AddAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public void HandleAsync_ShouldThrow_WhenUserRepositoryThrows()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new CreateAccountCommand { CustomerId = customerId };
            _userRepository.GetAsync(customerId, Arg.Any<CancellationToken>()).Throws(new Exception("db error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _handler.HandleAsync(command, CancellationToken.None));
        }
    }
}

