using Application.Errors;
using FluentResults;

namespace Application.Services.Utils;

public static class ErrorToResultBaseConvertor
{
    public static TRes ToResult<TRes>(this BaseApplicationError error) where TRes : ResultBase, new()
    {
        var fail = new TRes();
        fail.Reasons.Add(error);
        return fail;
    }
}