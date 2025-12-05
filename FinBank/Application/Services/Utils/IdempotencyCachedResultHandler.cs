using System.Text.Json;
using Application.DTOs;
using Application.Errors;
using Domain;
using FluentResults;

namespace Application.Services.Utils;

internal static class IdempotencyCachedResultHandler<TRes>
    where TRes : ResultBase, new()
{
    public static TRes FromRecord(IdempotencyKey record)
    {
        if (string.IsNullOrWhiteSpace(record.ResponseJson))
            return new TRes();

        try
        {
            var dto = JsonSerializer.Deserialize<TransferResultDto>(record.ResponseJson);
            if (dto is null)
                return new UnexpectedError("Cached response deserialized as null.").ToResult<TRes>();

            return (TRes)(ResultBase)Result.Ok(dto.Value);
        }
        catch (Exception ex) when (ex is JsonException or ArgumentNullException or NotSupportedException)
        {
            return new UnexpectedError(ex.Message).ToResult<TRes>();
        }
    }
}