using FluentValidation;
using Mediator.Abstractions;

namespace Application;
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    :IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest input, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        var context = new ValidationContext<TRequest>(input);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
