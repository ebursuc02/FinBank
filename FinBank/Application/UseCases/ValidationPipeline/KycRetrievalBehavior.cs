using Application.Errors;
using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using Domain.Kyc;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.ValidationPipeline;

public sealed class KycRetrievalBehavior<TReq, TRes>(
    IRiskClient riskClient,
    IUserRepository userRepository,
    IRiskContext riskContext) : IPipelineBehavior<TReq, TRes>
    where TRes : ResultBase, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken cancellationToken)
    {
        if (request is not CreateTransferCommand cmd) return await next();

        var userCnp = await userRepository.GetCustomerCnpByIdAsync(cmd.CustomerId, cancellationToken);
        var riskResult = userCnp is not null
            ? await riskClient.GetAsync(userCnp, cancellationToken)
            : Result.Fail(new NotFoundError("User not found"));
        var riskStatus = riskResult.IsSuccess ? riskResult.Value : RiskStatus.Medium;
        
        riskContext.Current = riskStatus;
        
        return await next();
    }
}