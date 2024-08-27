using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.BuisnessLogic.Edgar.Api;
internal interface IFundApi: IUseMemberToken
{
    [Get("/terminal/fund")]
    public Task<FundsResponse> GetFunds([AliasAs("post_id")] Guid postId);

    [Get("/terminal/fund/seizure-reasons")]
    public Task<SeizureReasonResponse[]> GetSeizureReasons();

    [Get("/terminal/fund/contribution-reasons")]
    public Task<ContributionReasonResponse[]> GetContributionReasons();

    [Post("/terminal/fund/contribution")]
    public Task Contribute(ContributeRequest contribute);

    [Post("/terminal/fund/seizure")]
    public Task Seizure(SeizureRequest seizure);
}

internal sealed record FundsResponse([property: JsonPropertyName("funds")] double? Funds);
internal sealed record SeizureReasonResponse([property: JsonPropertyName("depositreason_id")] Guid Id, [property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("isRequiredComment")] bool IsRequiredComment);
internal sealed record ContributionReasonResponse([property: JsonPropertyName("depositreason_id")] Guid Id, [property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("isRequiredComment")] bool IsRequiredComment);
internal sealed record ContributeRequest(
    [property: JsonPropertyName("post_id")] Guid CashierShiftId,
    [property: JsonPropertyName("comment")] string Comment,
    [property: JsonPropertyName("sum")] double Amount,
    [property: JsonPropertyName("memberId")] Guid MemberId,
    [property: JsonPropertyName("pincode")] string Pincode,
    [property: JsonPropertyName("depositreason_id")] Guid ContributionId);
internal sealed record SeizureRequest(
    [property: JsonPropertyName("post_id")] Guid CashierShiftId,
    [property: JsonPropertyName("comment")] string Comment,
    [property: JsonPropertyName("sum")] double Amount,
    [property: JsonPropertyName("memberId")] Guid MemberId,
    [property: JsonPropertyName("pincode")] string Pincode,
    [property: JsonPropertyName("withdrawalreason_id")] Guid SeizureId);