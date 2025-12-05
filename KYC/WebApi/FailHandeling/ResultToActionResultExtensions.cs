using Application.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.FailHandeling;

public static class ResultExtensions
{
    public static IActionResult? ToErrorResponseOrNull(
        this Result result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return null;

        var error = result.Errors[0];

        return error switch
        {
            NotFoundError     => controller.NotFound(new { error = error.Message }),
            ValidationError   => controller.BadRequest(new { error = error.Message }),
            ConflictError     => controller.Conflict(new { error = error.Message }),
            ForbiddenError    => controller.Forbid(),
            UnauthorizedError => controller.Unauthorized(new { error = error.Message }),

            ExternalServiceError => controller.StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new { error = error.Message }),

            UnexpectedError => controller.StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = "Unexpected internal error", detail = error.Message }),

            _ => controller.StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = "Unhandled error type", detail = error.Message })
        };
    }
    
    public static IActionResult? ToErrorResponseOrNull<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        return result.IsFailed
            ? Result.Fail(result.Errors).ToErrorResponseOrNull(controller)
            : null;
    }
}