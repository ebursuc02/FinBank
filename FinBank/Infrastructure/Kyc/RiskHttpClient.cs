using System.Net.Http.Json;
using Application.DTOs;
using Application.Interfaces.Kyc;
using Domain.Enums;
using FluentResults;

namespace Infrastructure.Kyc;

internal sealed class RiskHttpClient(HttpClient http) : IRiskClient
{
    public async Task<Result<RiskStatus>> GetAsync(Guid customerId, CancellationToken ct)
    {
        var resp = await http.GetFromJsonAsync<RiskRecordDto>($"/api/v1/kyc/{customerId}", ct);
        
        if(resp == null) return Result.Fail("Empty KYC response");

        var state = resp.RiskStatus.ToUpperInvariant() switch
        {
            "LOW" => RiskStatus.Low, "MEDIUM" => RiskStatus.Medium,
            "HIGH" => RiskStatus.High, "BLOCKED" => RiskStatus.Blocked,
            _ => RiskStatus.Medium
        };

        return state;
    }
}
