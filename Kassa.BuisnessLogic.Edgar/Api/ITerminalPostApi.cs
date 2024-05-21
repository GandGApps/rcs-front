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
    public Task<PostExistsResponse> PostExists(PostExistsRequest postExists);

    [Post("/terminal/open-post")]
    public Task<OpenPostRequest> OpenPost(TerminalOpenPostRequest request);

    [Post("/terminal/close-post")]
    public Task ClosePost(TerminalClosePostRequest request);
}

internal sealed record TerminalClosePostRequest([property: JsonPropertyName("closeDate")] DateTime CloseDate, [property: JsonPropertyName("post_id")] Guid PostId);

internal sealed record TerminalOpenPostRequest([property: JsonPropertyName("openDate")] DateTime OpenDate, [property: JsonPropertyName("post_id")] Guid PostId, [property: JsonPropertyName("start_sum")] double StartSum);