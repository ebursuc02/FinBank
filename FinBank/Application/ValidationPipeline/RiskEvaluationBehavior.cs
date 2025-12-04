using Application.Errors;
using Application.Interfaces.Kyc;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands;
using Application.UseCases.Commands.TransferCommands;
using Domain.Enums;
using FluentResults;
using Mediator.Abstractions;

namespace Application.ValidationPipeline;

public sealed class RiskEvaluationBehavior<TReq, TRes>(
    IRiskClient riskClient,
    IRiskPolicyEvaluator evaluator,
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

        var decision = evaluator.Evaluate(riskStatus, out var reason);
        var policyVersion = string.IsNullOrWhiteSpace(cmd.PolicyVersion) ? "v1" : cmd.PolicyVersion;

        riskContext.Current = new RiskContextData(decision, reason, policyVersion);

        return await next();
    }
}