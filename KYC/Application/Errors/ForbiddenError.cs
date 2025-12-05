namespace Application.Errors;

public sealed class ForbiddenError(string message = "Forbidden") : BaseApplicationError("FORBIDDEN", message);