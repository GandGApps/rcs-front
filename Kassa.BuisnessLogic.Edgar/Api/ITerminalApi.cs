using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.BuisnessLogic.Edgar.Api;

internal interface ITerminalApi: IUseTerminalToken
{
    [Post("/terminal/login-terminal")]
    public Task<IApiResponse<string>> Login(LoginTerminalRequest request);

    [Post("/terminal/login-employee")]
    public Task<IApiResponse<PincodeResponse>> EnterPincode(LoginEmployeeRequest request);

    [Post("/terminal/is-manager-pincode")]
    public Task<IApiResponse<string>> IsManagerPincode(EnterPincodeRequest request);
}

internal sealed record LoginTerminalRequest(string Login, string Password);
internal sealed record EnterPincodeRequest([property:JsonPropertyName("pin_code")] string Pincode);
internal sealed record LoginEmployeeRequest([property: JsonPropertyName("pin_code")] string Pincode, [property: JsonPropertyName("date")] DateTime DateTime);
internal sealed record PincodeResponse(
    [property: JsonPropertyName("token")] string Token);
