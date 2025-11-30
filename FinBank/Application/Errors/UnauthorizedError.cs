namespace Application.Errors;

public sealed class UnauthorizedError(string message = "Unauthorized") : BaseApplicationError("UNAUTHORIZED", message);