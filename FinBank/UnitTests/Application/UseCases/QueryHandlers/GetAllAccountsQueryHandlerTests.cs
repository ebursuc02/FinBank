using System.Collections.Generic;
using System.Linq;
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
    public class GetAllAccountsQueryHandlerTests
    {
        private IAccountRepository _repository;
        private IMapper _mapper;
        private GetAllAccountsQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IAccountRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetAllAccountsQueryHandler(_repository, _mapper);
        }

        [Test]
        public async Task HandleAsync_ShouldReturnAccounts_ForValidCustomerWithOpenAccounts()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var accounts = new List<Account>
            {
                new Account { IsClosed = false },
                new Account { IsClosed = false }
            };
    
            _repository.GetByCustomerAsync(customerId, Arg.Any<CancellationToken>())
                .Returns(accounts);

            _mapper.Map<AccountDto>(Arg.Any<Account>())
                .Returns(callInfo => new AccountDto
                {
                    Iban = Guid.NewGuid().ToString(),
                    Currency = "EUR"
                });

            var query = new GetAllAccountsQuery { CustomerId = customerId };

            // Act
            var result = await _handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Value.Count(), Is.EqualTo(2));
                _repository.Received(1)
                    .GetByCustomerAsync(customerId, Arg.Any<CancellationToken>());
            });
        }


        [Test]
        public async Task HandleAsync_ShouldReturnNotFound_WhenNoOpenAccounts()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var accounts = new List<Account>
            {
                new Account { IsClosed = true }
            };
            _repository.GetByCustomerAsync(customerId, Arg.Any<CancellationToken>()).Returns(accounts);
            var query = new GetAllAccountsQuery { CustomerId = customerId };

            // Act
            var result = await _handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors.First(), Is.TypeOf<NotFoundError>());
                _repository.Received(1).GetByCustomerAsync(customerId, Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public async Task HandleAsync_ShouldReturnNotFound_WhenNoAccountsAtAll()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var accounts = new List<Account>();
            _repository.GetByCustomerAsync(customerId, Arg.Any<CancellationToken>()).Returns(accounts);
            var query = new GetAllAccountsQuery { CustomerId = customerId };

            // Act
            var result = await _handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.Errors.First(), Is.TypeOf<NotFoundError>());
                _repository.Received(1).GetByCustomerAsync(customerId, Arg.Any<CancellationToken>());
            });
        }

        [Test]
        public void HandleAsync_ShouldReturnFailed_WhenRepositoryThrows()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _repository.GetByCustomerAsync(customerId, Arg.Any<CancellationToken>()).Throws(new Exception("db error"));
            var query = new GetAllAccountsQuery { CustomerId = customerId };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _handler.HandleAsync(query, CancellationToken.None));
        }
    }
}

