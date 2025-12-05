using Application.DTOs;
using Application.DTOs.Enums;
using Application.Interfaces.Repositories;
using Application.UseCases.CommandHandlers;
using Application.UseCases.Commands;
using AutoMapper;
using Domain;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;


namespace UnitTests.Application.UseCases.CommandHandlers
{
    [TestFixture]
    public class GetRiskStatusCommandHandlerTests
    {
        private IUserRiskRepository _userRiskRepository;
        private IMapper _mapper;
        private GetRiskStatusCommandHandler _handler;

        
        [SetUp]
        public void SetUp()
        {
            _userRiskRepository = Substitute.For<IUserRiskRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetRiskStatusCommandHandler(_userRiskRepository, _mapper);
        }
        
        [Test]
        public async Task Handle_ShouldReturnUserRiskDto_ForValidCommand()
        {
            // Arrange
            var userCnp = "1234567890123";
            var command = new GetRiskStatusCommand { UserCnp = userCnp };
            var customerRisk = new CustomerRisk
            {
                Cnp = userCnp,
                RiskStatus = Domain.Enums.RiskStatus.Low,
                UpdatedAt = DateTime.UtcNow
            };
            var userRiskDto = new UserRiskDto
                { Cnp = userCnp, RiskStatus = RiskStatusDto.Low, UpdatedAt = DateTime.UtcNow };

            _userRiskRepository.GetByCustomerByCnpAsync(userCnp, Arg.Any<CancellationToken>()).Returns(customerRisk);
            _mapper.Map<UserRiskDto>(customerRisk).Returns(userRiskDto);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess);
                Assert.That(result.Value, Is.EqualTo(userRiskDto));
            });
        }

        [Test]
        public async Task Handle_ShouldReturnErrorResult_WhenCustomerNotFound()
        {
            // Arrange
            var userCnp = "1234567890123";
            var command = new GetRiskStatusCommand { UserCnp = userCnp };
            _userRiskRepository.GetByCustomerByCnpAsync(userCnp, Arg.Any<CancellationToken>())
                .ReturnsNull();

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed);
                Assert.That(result.ValueOrDefault, Is.Null);
            });
        }

        [Test]
        public async Task Handle_ShouldCallRepositoryAndMapper()
        {
            // Arrange
            var userCnp = "1234567890123";
            var command = new GetRiskStatusCommand { UserCnp = userCnp };
            var customerRisk = new CustomerRisk
            {
                Cnp = userCnp,
                RiskStatus = Domain.Enums.RiskStatus.High,
                UpdatedAt = DateTime.UtcNow
            };
            var userRiskDto = new UserRiskDto
                { Cnp = userCnp, RiskStatus = RiskStatusDto.High, UpdatedAt = DateTime.UtcNow };

            _userRiskRepository.GetByCustomerByCnpAsync(userCnp, Arg.Any<CancellationToken>()).Returns(customerRisk);
            _mapper.Map<UserRiskDto>(customerRisk).Returns(userRiskDto);

            // Act
            await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            await _userRiskRepository.Received(1).GetByCustomerByCnpAsync(userCnp, Arg.Any<CancellationToken>());
            _mapper.Received(1).Map<UserRiskDto>(customerRisk);
        }

        [Test]
        public async Task Handle_ShouldReturnFailureResult_OnException()
        {
            // Arrange
            var userCnp = "1234567890123";
            var command = new GetRiskStatusCommand { UserCnp = userCnp };
            _userRiskRepository.GetByCustomerByCnpAsync(userCnp, Arg.Any<CancellationToken>())!
                .Returns<Task<CustomerRisk>>(_ => throw new Exception("DB error"));

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailed, Is.True);
                Assert.That(result.Errors.Any(e => e.Message.Contains("DB error")));
            });
        }
    }
}