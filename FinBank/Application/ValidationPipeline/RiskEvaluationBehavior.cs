using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public class RiskEvaluationBehavior<TReq,TRes> : IPipelineBehavior<TReq,TRes>
{
    public Task<TRes> HandleAsync(TReq input, Func<Task<TRes>> next, CancellationToken cancellationToken)
    {
        return next();
    }
}