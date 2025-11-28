Guidelines for Application Layer Unit Tests
1. Test Class Structure
   Name test classes as {ClassUnderTest}Tests (e.g., GetRiskStatusCommandHandlerTests).
   Place tests in a corresponding namespace, e.g., UnitTests.Application.UseCases.CommandHandlers.
2. Test Setup
   Use [SetUp] to initialize mocks and the system under test.
   Use mocking frameworks (e.g., NSubstitute) for dependencies.
   Avoid real database or external service calls.
3. Test Naming
   Name test methods to clearly describe the scenario and expected outcome, e.g., Handle_ShouldReturnUserRiskDto_ForValidCommand.
4. Test Coverage
   Cover all logical branches:
   Success scenarios (valid input, expected output).
   Failure scenarios (e.g., not found, exceptions).
   Edge cases (nulls, empty collections, etc.).
   Verify both the result and side effects (e.g., method calls on dependencies).
5. Arrange-Act-Assert Pattern
   Arrange: Set up test data, mocks, and expectations.
   Act: Call the method under test.
   Assert: Verify the result and interactions.
6. Assertions
   Use Assert.Multiple for grouped assertions.
   Check both result status (e.g., IsSuccess, IsFailed) and returned values.
   Assert that dependencies are called with correct arguments and expected number of times.
7. Exception Handling
   Simulate exceptions in dependencies and verify the handler returns a failure result with the correct error message.
8. Isolation
   Each test should be independent and not rely on shared state.
   Avoid static/shared variables unless absolutely necessary.
9. Readability & Maintainability
   Keep tests concise and focused on a single behavior.
   Use meaningful variable names.
   Add comments only where necessary for clarity.
10. Consistency
    Follow a consistent structure and naming convention across all tests.
    Group related tests together.
<hr></hr>
Example Test Skeleton
[TestFixture]
public class ExampleHandlerTests
{
    private IDependency _dependency;
    private ExampleHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _dependency = Substitute.For<IDependency>();
        _handler = new ExampleHandler(_dependency);
    }

    [Test]
    public async Task Handle_ShouldReturnExpectedResult_ForValidInput()
    {
        // Arrange
        // ...setup...

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess);
            // ...other assertions...
        });
    }

    // ...other tests...
}
