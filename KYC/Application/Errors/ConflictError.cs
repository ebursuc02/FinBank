namespace Application.Errors;

public sealed class ConflictError(string message) : BaseApplicationError("CONFLICT", message);