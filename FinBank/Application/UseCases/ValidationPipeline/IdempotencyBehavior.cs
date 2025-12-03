using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Errors;
using Application.Interfaces.Repositories;
using Application.Interfaces.Utils;
using Domain;
using FluentResults;
using Mediator.Abstractions;

namespace Application.UseCases.ValidationPipeline;

public class IdempotencyBehavior<TReq, TRes>(
    IIdempotencyRepository repository
) : IPipelineBehavior<TReq, TRes>
    where TRes : ResultBase, new()
{
    public async Task<TRes> HandleAsync(
        TReq request,
        Func<Task<TRes>> next,
        CancellationToken ct)
    {
        if (request is not IIdempotencyCheckable idempotencyCheckable)
            return await next();

        var record = await repository.GetAsync(idempotencyCheckable.IdempotencyKey, ct);
        
        if (record is not null)
        {
            if (string.IsNullOrWhiteSpace(record.ResponseJson)) return new TRes();
            try
            {
                var deserialized = JsonSerializer.Deserialize<TRes>(record.ResponseJson);
                if (deserialized is not null)
                    return deserialized;
            }
            catch (Exception ex) when (ex is JsonException or ArgumentNullException or NotSupportedException)
            {
                return Fail(new UnexpectedError(ex.Message));
            }
            return new TRes();
        }
        
        var result = await next();
        
        var requestJson = JsonSerializer.Serialize(request);
        var requestHash = ComputeSha256Base64(requestJson);
        
        var responseJson = JsonSerializer.Serialize(result);

        var ik = new IdempotencyKey
        {
            FirstProcessedAt = DateTime.UtcNow,
            IdempotencyKeyValue = idempotencyCheckable.IdempotencyKey,
            RequestHash = requestHash,
            ResponseJson = responseJson
        };

        await repository.AddAsync(ik, ct);

        return result;
    }

    private static string ComputeSha256Base64(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }
    
    private static TRes Fail(BaseApplicationError  error)
    {
        var fail = new TRes();
        fail.Reasons.Add(error);
        return fail;
    }
}
