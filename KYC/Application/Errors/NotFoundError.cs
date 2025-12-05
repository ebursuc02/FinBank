namespace Application.Errors;

public sealed class NotFoundError(string message) : BaseApplicationError("NOT_FOUND", message);