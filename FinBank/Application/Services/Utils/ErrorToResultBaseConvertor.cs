using Application.Errors;
using FluentResults;

namespace Application.Services.Utils;

public static class ErrorToResultBaseConvertor<TRes> where TRes : ResultBase, new()
{
    public static TRes Fail(BaseApplicationError  error)
    {
        var fail = new TRes();
        fail.Reasons.Add(error);
        return fail;
    }
}