using Application.Interfaces.Kyc;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using Domain.Kyc;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.ValidationPipeline;

public sealed class KycRetrievalBehavior<TReq, TRes>(
    IRiskClient riskClient,
    IRiskContext riskContext) : IPipelineBehavior<TReq, TRes>
    where TRes : ResultBase
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken cancellationToken)
    {
        if (request is not CreateTransferCommand cmd) return await next();
        
        var riskResult = await riskClient.GetAsync(cmd.CustomerId, cancellationToken) ;
        var riskStatus = riskResult.IsSuccess ? riskResult.Value : RiskStatus.Medium;
        
        riskContext.Current = riskStatus;
        
        return await next();
    }
}