namespace Application.Errors;

public sealed class UnexpectedError(string message = "Unexpected internal error")
    : BaseApplicationError("UNEXPECTED_ERROR", message);