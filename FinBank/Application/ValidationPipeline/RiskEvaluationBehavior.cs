using Application.Interfaces.Kyc;
using Application.UseCases.Commands;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class RiskEvaluationBehavior<TReq, TRes>(
    IRiskClient riskClient,
    IRiskPolicyEvaluator evaluator,
    IRiskContext riskContext) : IPipelineBehavior<TReq, TRes>
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not CreateTransferCommand cmd) return await next();
        
        var riskResult = await riskClient.GetAsync(cmd.CustomerId, cancellationToken) ;
        var riskStatus = riskResult.IsSuccess ? riskResult.Value : RiskStatus.Medium;
        
        var decision = evaluator.Evaluate(riskStatus, out var reason);
        var policyVersion = string.IsNullOrWhiteSpace(cmd.PolicyVersion) ? "v1" : cmd.PolicyVersion;
        
        riskContext.Current = new RiskContextData(decision, reason, policyVersion);
        
        try
        {
            return await next();
        }
        finally
        {
            riskContext.Clear();
        }
    }
}