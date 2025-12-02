using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.UseCases.Queries;
using Application.UseCases.QueryHandlers;
using AutoMapper;
using Domain;
using FluentResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace UnitTests.Application.UseCases.QueryHandlers
{
    [TestFixture]
    public class GetAccountQueryHandlerTests
    {
        private IAccountRepository _repository;
        private IMapper _mapper;
        private GetAccountQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IAccountRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetAccountQueryHandler(_repository, _mapper);
        }

        [Test]
        public async Task HandleAsync_ShouldReturnAccount_ForValidIban()
        {
            // Arrange
            var account = new Domain.Account { IsClosed = false };
            _repository.GetByIbanAsync("iban1", Arg.Any<CancellationToken>()).Returns(account);

            _mapper.Map<AccountDto>(account).Returns(new AccountDto
            {
                Iban = "iban1",
                Currency = "EUR"
            });

            var query = new GetAccountQuery { AccountIban = "iban1" };

            // Act
            var result = await _handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Value, Is.Not.Null);
                _repository.Received(1).GetByIbanAsync("iban1", Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            _repository.GetByIbanAsync("iban2", Arg.Any<CancellationToken>()).Returns((Account)null);
            var query = new GetAccountQuery { AccountIban = "iban2" };

            // Act
            var result = await _handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors[0], Is.TypeOf<NotFoundError>());
                Assert.That(result.Errors[0].Metadata["StatusCode"], Is.EqualTo(HttpStatusCode.NotFound));
                _repository.Received(1).GetByIbanAsync("iban2", Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnNotFound_WhenAccountIsClosed()
        {
            // Arrange
            var account = new Domain.Account { IsClosed = true };
            _repository.GetByIbanAsync("iban3", Arg.Any<CancellationToken>()).Returns(account);
            var query = new GetAccountQuery { AccountIban = "iban3" };

            // Act
            var result = await _handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors[0], Is.TypeOf<NotFoundError>());
                _repository.Received(1).GetByIbanAsync("iban3", Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public void HandleAsync_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            _repository.GetByIbanAsync("iban4", Arg.Any<CancellationToken>()).Throws(new Exception("db error"));
            var query = new GetAccountQuery { AccountIban = "iban4" };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _handler.HandleAsync(query, CancellationToken.None));
        }
    }
}

