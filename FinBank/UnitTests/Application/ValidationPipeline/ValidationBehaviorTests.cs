using Application.ValidationPipeline;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

namespace UnitTests.Application.ValidationPipeline;

[TestFixture]
public class ValidationBehaviorTests
{
    private IValidator<TestCommand> _validator;
    private ValidationBehavior<TestCommand, Result> _behavior;
    private Func<Task<Result>> _next;

    public class TestCommand
    {
        public string Name { get; set; }
    }

    public class OtherCommand
    {
    }

    [SetUp]
    public void SetUp()
    {
        _validator = Substitute.For<IValidator<TestCommand>>();
        _behavior = new ValidationBehavior<TestCommand, Result>(new[] { _validator });
        _next = Substitute.For<Func<Task<Result>>>();
    }

    [Test]
    public async Task Should_BlockCommandExecution_AndReturnValidationErrors_ForInvalidCommand()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };
        _validator.Validate(Arg.Any<ValidationContext<TestCommand>>()).Returns(new ValidationResult(failures));
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(new TestCommand(), _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors.Any(e => e.Message.Contains("Name is required")), Is.True);
            _next.DidNotReceive()();
        });
    }

    [Test]
    public async Task Should_AllowValidCommands_ToProceedToHandler()
    {
        // Arrange
        _validator.Validate(Arg.Any<ValidationContext<TestCommand>>()).Returns(new ValidationResult());
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await _behavior.HandleAsync(new TestCommand(), _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            _next.Received(1)();
        });
    }

    [Test]
    public async Task Should_CallCorrectValidator_ForEachCommandType()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<TestCommand>>();
        var validator2 = Substitute.For<IValidator<OtherCommand>>();
        var behavior = new ValidationBehavior<TestCommand, Result>(new[] { validator1 });
        validator1.Validate(Arg.Any<ValidationContext<TestCommand>>()).Returns(new ValidationResult());
        _next.Invoke().Returns(Result.Ok());

        // Act
        var result = await behavior.HandleAsync(new TestCommand(), _next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            validator1.Received(1).Validate(Arg.Any<ValidationContext<TestCommand>>());
            validator2.DidNotReceive().Validate(Arg.Any<ValidationContext<OtherCommand>>());
        });
    }
}