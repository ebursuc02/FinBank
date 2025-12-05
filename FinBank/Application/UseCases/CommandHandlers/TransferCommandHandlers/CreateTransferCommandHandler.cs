using Application.DTOs;
using FluentResults;
using Application.Interfaces.Repositories;
using Application.UseCases.Commands.TransferCommands;
using AutoMapper;
using Domain;
using Domain.Kyc;
using Mediator.Abstractions;

namespace Application.UseCases.CommandHandlers.TransferCommandHandlers;

public sealed class CreateTransferCommandHandler(
    IRiskContext riskContext,
    IRiskPolicyEvaluator evaluator,
    ITransferRepository transferRepository) : ICommandHandler<CreateTransferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(CreateTransferCommand cmd, CancellationToken ct)
    {
        var kycDecisionContext = evaluator.Evaluate(riskContext.Current, out var reason);

        var transfer = new Transfer
        {
            TransferId = Guid.NewGuid(),
            FromIban = cmd.Iban,
            ToIban = cmd.ToIban,
            Amount = decimal.Round(cmd.Amount, 2, MidpointRounding.ToEven),
            Currency = cmd.Currency,
            Status = kycDecisionContext,
            Reason = reason,
            PolicyVersion = cmd.PolicyVersion ?? "v1",
            CreatedAt = DateTime.UtcNow,
        };

        await transferRepository.AddAsync(transfer, ct);
        return transfer.TransferId;
    }
}