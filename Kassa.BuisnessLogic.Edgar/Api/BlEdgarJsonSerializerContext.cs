using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Edgar.Api;

[JsonSerializable(typeof(ClosePostRequest))]
[JsonSerializable(typeof(BreakStartRequest))]
[JsonSerializable(typeof(EmployeeGetPostsRequest))]
[JsonSerializable(typeof(IEnumerable<EmployeeResponsePost>))]
[JsonSerializable(typeof(EmployeeOpenPostRequest))]
[JsonSerializable(typeof(IEnumerable<OpenPostRequest>))]
[JsonSerializable(typeof(EmployeeClosePostRequest))]
[JsonSerializable(typeof(PostExistsResponse))]
[JsonSerializable(typeof(PostExistsRequest))]
[JsonSerializable(typeof(FundsResponse))]
[JsonSerializable(typeof(SeizureReasonResponse[]))]
[JsonSerializable(typeof(ContributionReasonResponse[]))]
[JsonSerializable(typeof(ContributeRequest))]
[JsonSerializable(typeof(SeizureRequest))]
[JsonSerializable(typeof(LoginTerminalRequest))]
[JsonSerializable(typeof(LoginEmployeeRequest))]
[JsonSerializable(typeof(PincodeResponse))]
[JsonSerializable(typeof(EnterPincodeRequest))]
[JsonSerializable(typeof(TerminalPostExistsResponse))]
[JsonSerializable(typeof(TerminaPostExistsRequest))]
[JsonSerializable(typeof(TerminalOpenPostRequest))]
[JsonSerializable(typeof(TerminalClosePostRequest))]
internal sealed partial  class BlEdgarJsonSerializerContext : JsonSerializerContext
{
}
