using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.BuisnessLogic.Edgar.Api;
internal interface ITerminalPostApi: IUseMemberToken
{
    [Post("/terminal/post-exists")]
    public Task<TerminalPostExistsResponse> PostExists(TerminaPostExistsRequest postExists);

    [Post("/terminal/open-post")]
    public Task<OpenPostRequest> OpenPost(TerminalOpenPostRequest request);

    [Post("/terminal/close-post")]
    public Task ClosePost(TerminalClosePostRequest request);
}

internal sealed record TerminaPostExistsRequest([property: JsonPropertyName("openDate")] DateTime CurrentDate);

internal sealed record TerminalClosePostRequest([property: JsonPropertyName("closeDate")] DateTime CloseDate, [property: JsonPropertyName("post_id")] Guid PostId);

internal sealed record TerminalOpenPostRequest([property: JsonPropertyName("openDate")] DateTime OpenDate, [property: JsonPropertyName("post_id")] Guid PostId, [property: JsonPropertyName("start_sum")] double StartSum);

internal sealed record TerminalPostExistsResponse
{
    [JsonPropertyName("exists")]
    public bool Exists
    {
        get; init;
    }

    [JsonPropertyName("posts")]
    public TerminalPostDetails Posts
    {
        get; init;
    }

    [JsonPropertyName("post_id")]
    public Guid PostId
    {
        get; init;
    }
}

internal sealed record TerminalPostDetails
{
    [JsonPropertyName("post_id")]
    public Guid PostId
    {
        get; init;
    }

    [JsonPropertyName("terminal_id")]
    public Guid TerminalId
    {
        get; init;
    }

    [JsonPropertyName("openDate")]
    public DateTime? OpenDate
    {
        get; init;
    }

    [JsonPropertyName("closeDate")]
    public DateTime? CloseDate
    {
        get; init;
    }

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; init;
    }

    [JsonPropertyName("isOpen")]
    public bool IsOpen
    {
        get; init;
    }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt
    {
        get; init;
    }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt
    {
        get; init;
    }
}