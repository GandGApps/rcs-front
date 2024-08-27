using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IFundApi: IUseMemberToken
{
    [Get("/terminal/fund/contribution-reasons")]
    public Task<ContributionResponse[]> GetContributions();

    [Get("/terminal/fund/seizure-reason")]
    public Task<SeizureResponse[]> GetSeizures();
}

internal sealed record ContributionResponse(
    [property: JsonPropertyName("depositreason_id")] Guid Id,
    [property: JsonPropertyName("name")]  string? Name,
    [property: JsonPropertyName("isRequiredComment")] bool? IsRequiredComment);

internal sealed record SeizureResponse(
    [property: JsonPropertyName("withdrawalreason_id")] Guid Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("isRequiredComment")] bool? IsRequiredComment);
