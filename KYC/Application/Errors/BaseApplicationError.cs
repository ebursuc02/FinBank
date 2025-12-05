using FluentResults;

namespace Application.Errors;

public abstract class BaseApplicationError : Error
{
    public string Code { get; }

    protected BaseApplicationError(string code, string message) : base(message)
    {
        Code = code;
        Metadata.Add("ErrorCode", code);
    }

    public override string ToString()
    {
        return $"{Code}: {Message}";
    }
}