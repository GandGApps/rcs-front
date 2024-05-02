using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Kassa.BuisnessLogic.Edgar.Api;

internal interface ITerminalApi
{
    [Post("/terminal/login-terminal")]
    public Task<IApiResponse<string>> Login(LoginTerminalRequest request);

    [Post("/terminal/login-employee")]
    public Task<IApiResponse<string>> EnterPincode(EnterPincodeRequest request);

    [Post("/terminal/is-manager-pincode")]
    public Task<IApiResponse<string>> IsManagerPincode(EnterPincodeRequest request);
}

internal sealed record LoginTerminalRequest(string Username, string Password);
internal sealed record EnterPincodeRequest([AliasAs("pin_code")] string Pincode);
