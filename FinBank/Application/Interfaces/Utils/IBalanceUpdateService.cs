using Domain;
using FluentResults;

namespace Application.Interfaces.Utils;

public interface IBalanceUpdateService
{
    Task<Result> UpdateBalance(Transfer transfer, CancellationToken ct);
}