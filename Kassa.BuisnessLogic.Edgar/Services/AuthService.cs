using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed partial class AuthService : IAuthService, IEnableLogger
{
    private readonly AdapterBehaviorSubject<IAuthenticationContext> _adapterBehaviorSubject = new(JwtAuthenticationContext.NotAuthenticated);
    private readonly IConfiguration _configuration;
    private readonly ITerminalApi _terminalApi;
    private readonly IMemberService _memberService;

    public AuthService(IConfiguration configuration, ITerminalApi terminalApi, IMemberService memberService)
    {
        _configuration = configuration;
        _terminalApi = terminalApi;
        _memberService = memberService;
    }

    public IObservableOnlyBehaviourSubject<IAuthenticationContext> CurrentAuthenticationContext => _adapterBehaviorSubject;

    public bool IsAuthenticated => _adapterBehaviorSubject.Value.IsAuthenticated;

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        var loginRequest = new LoginTerminalRequest(username, password);

        var response = await _terminalApi.Login(loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var token = response.Content!;
            token = token.Trim('"');
            _configuration["TerminalAuthToken"] = token;

            _adapterBehaviorSubject.OnNext(new JwtAuthenticationContext
            {
                Token = token
            });

            return true;
        }

        return false;
    }

    public async Task<bool> LogoutAsync()
    {
        await RcsKassa.DisposeScope();
        _adapterBehaviorSubject.OnNext(JwtAuthenticationContext.NotAuthenticated);
        return true;
    }

    public async Task<bool> IsManagerPincode(string pincode)
    {
        var pincodeRequest = new EnterPincodeRequest(pincode);

        var response = await _terminalApi.IsManagerPincode(pincodeRequest);

        return response.IsSuccessStatusCode;
    }

    public ValueTask<bool> LogoutAccount()
    {
        var context = _adapterBehaviorSubject.Value;

        if (context is JwtAuthenticationContext jwtContext)
        {
            jwtContext.Member = null;

            _adapterBehaviorSubject.OnNext(jwtContext);

            return new(true);
        }
        else
        {
            this.Log().Warn("Trying to logout from not authenticated account");
            ThrowHelper.ThrowInvalidOperationException("Trying to logout from not authenticated account");
        }
    }

    public async Task<bool> EnterPincode(string pincode)
    {
        var pincodeRequest = new LoginEmployeeRequest(pincode, DateTime.UtcNow);

        var reponse = await _terminalApi.EnterPincode(pincodeRequest);

        if (reponse.IsSuccessStatusCode)
        {
            var pincodeResponse = reponse.Content!;

            _configuration["MemberAuthToken"] = pincodeResponse.Token;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(pincodeResponse.Token);

            var employeeId = token.Claims.First(claim => claim.Type == "employee_id").Value;

            if (!_memberService.IsInitialized)
            {
                await _memberService.Initialize();
            }

            var member = await _memberService.GetMember(Guid.Parse(employeeId));

            var jwtContext = Unsafe.As<JwtAuthenticationContext>(_adapterBehaviorSubject.Value);

            jwtContext.Member = member;

            _adapterBehaviorSubject.OnNext(jwtContext);

            return true;
        }

        return false;
    }

    public async Task<bool> CheckPincode(MemberDto member, string pincode)
    {
        var result = await _terminalApi.CheckPincode(pincode, member.Id);

        return result.Content;
    }
}
