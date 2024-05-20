using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.BuisnessLogic.Edgar.Api;


internal interface IEmployeeApi: IUseMemberToken
{

    [Post("/employee/close-post")]
    public Task ClosePost(ClosePostRequest request);

    [Post("/employee/break-start")]
    public Task BreakStart(BreakStartRequest request);
}

internal sealed record ClosePostRequest([property: JsonPropertyName("closeDate")] DateTime CurrentDate, [property: JsonPropertyName("pincode")] string Pincode);

internal sealed record BreakStartRequest([property: JsonPropertyName("date")] DateTime CurrentDate, [property: JsonPropertyName("pincode")] string Pincode);