using FluentResults;
using FluentValidation;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result, new()
{
    public async Task<TResponse> HandleAsync(TRequest input, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(input);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();
        
        if (failures.Count == 0) return await next();
        
        var fail = new TResponse();
        fail.WithError(failures.ToString());
        return fail;
    }
}
