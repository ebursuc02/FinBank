using Application.Errors;
using FluentResults;

namespace Application.Services.Utils;

public static class ErrorToResultBaseConvertor
{
    public static ResultBase Fail(BaseApplicationError  error)
    {
        var fail = new Result();
        fail.Reasons.Add(error);
        return fail;
    }
}