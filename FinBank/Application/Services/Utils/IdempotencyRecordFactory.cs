using System.Text.Json;
using Domain;

namespace Application.Services.Utils;

internal static class IdempotencyRecordFactory
{
    public static IdempotencyKey Build<TReq, TRes>(
        Guid idempotencyKey,
        TReq request,
        TRes result)
    {
        var requestJson = JsonSerializer.Serialize(request);
        var requestHash = IdempotencyHashing.ComputeSha256Base64(requestJson);

        var responseJson = JsonSerializer.Serialize(result);

        return new IdempotencyKey
        {
            FirstProcessedAt = DateTime.UtcNow,
            IdempotencyKeyValue = idempotencyKey,
            RequestHash = requestHash,
            ResponseJson = responseJson
        };
    }
}
