namespace Application.Errors;

public sealed class ValidationError(string message) : BaseApplicationError("VALIDATION_ERROR", message);