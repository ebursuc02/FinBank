using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NUnit.Framework;
using Application;

namespace UnitTests.Application;

[TestFixture]
public class ValidationBehaviorTests
{
    private IValidator<TestCommand> _validator;
    private ValidationBehavior<TestCommand, string> _behavior;
    private Func<Task<string>> _next;

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
        _behavior = new ValidationBehavior<TestCommand, string>(new[] { _validator });
        _next = Substitute.For<Func<Task<string>>>();
    }

    [Test]
    public void Should_BlockCommandExecution_AndReturnValidationErrors_ForInvalidCommand()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };
        _validator.Validate(Arg.Any<ValidationContext<TestCommand>>()).Returns(new ValidationResult(failures));

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(async () =>
            await _behavior.HandleAsync(new TestCommand(), _next, CancellationToken.None));
        Assert.That(ex.Errors.Any(f => f.ErrorMessage == "Name is required"));
        _next.DidNotReceive()();
    }

    [Test]
    public async Task Should_AllowValidCommands_ToProceedToHandler()
    {
        // Arrange
        _validator.Validate(Arg.Any<ValidationContext<TestCommand>>()).Returns(new ValidationResult());
        _next.Invoke().Returns("success");

        // Act
        var result = await _behavior.HandleAsync(new TestCommand(), _next, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo("success"));
        await _next.Received(1)();
    }

    [Test]
    public void Should_CallCorrectValidator_ForEachCommandType()
    {
        // Arrange
        var validator1 = Substitute.For<IValidator<TestCommand>>();
        var validator2 = Substitute.For<IValidator<OtherCommand>>();
        var behavior = new ValidationBehavior<TestCommand, string>(new[] { validator1 });
        validator1.Validate(Arg.Any<ValidationContext<TestCommand>>()).Returns(new ValidationResult());
        _next.Invoke().Returns("ok");

        // Act
        Assert.DoesNotThrowAsync(async () =>
            await behavior.HandleAsync(new TestCommand(), _next, CancellationToken.None));
        validator1.Received(1).Validate(Arg.Any<ValidationContext<TestCommand>>());
        validator2.DidNotReceive().Validate(Arg.Any<ValidationContext<OtherCommand>>());
    }
}