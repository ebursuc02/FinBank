namespace Application.Errors;

public sealed class ExternalServiceError(string message) : BaseApplicationError("EXTERNAL_SERVICE_FAILURE", message);