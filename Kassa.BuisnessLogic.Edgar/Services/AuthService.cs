using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal partial class AuthService : IAuthService, IEnableLogger
{
    private readonly BehaviorSubject<IAuthenticationContext> _currentAuthenticationContext = new(JwtAuthenticationContext.NotAuthenticated);
    private readonly ObservableOnlyBehaviourSubject<IAuthenticationContext> _observableOnly;
    public AuthService()
    {
        _observableOnly = new(_currentAuthenticationContext);
    }

    public IObservableOnlyBehaviourSubject<IAuthenticationContext> CurrentAuthenticationContext => _observableOnly;

    public bool IsAuthenticated => _currentAuthenticationContext.Value.IsAuthenticated;

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        var loginRequest = new LoginTerminalRequest(username, password);
        var config = Locator.Current.GetRequiredService<IConfiguration>();

        var terminalApi = Locator.Current.GetRequiredService<ITerminalApi>();

        var response = await terminalApi.Login(loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var token = response.Content!;
            token = token.Trim('"');
            config["TerminalAuthToken"] = token;

            _currentAuthenticationContext.OnNext(new JwtAuthenticationContext
            {
                Token = token
            });

            return true;
        }

        return false;
    }

    public Task<bool> LogoutAsync()
    {
        _currentAuthenticationContext.OnNext(JwtAuthenticationContext.NotAuthenticated);
        return Task.FromResult(true);
    }

    public async Task<bool> IsManagerPincode(string pincode)
    {
        var pincodeRequest = new EnterPincodeRequest(pincode);

        var terminalApi = Locator.Current.GetRequiredService<ITerminalApi>();

        var response = await terminalApi.IsManagerPincode(pincodeRequest);

        return response.IsSuccessStatusCode;
    }

    public ValueTask<bool> LogoutAccount()
    {
        var context = _currentAuthenticationContext.Value;

        if (context is JwtAuthenticationContext jwtContext)
        {
            jwtContext.Member = null;

            _currentAuthenticationContext.OnNext(jwtContext);

            return new(true);
        }
        else
        {
            this.Log().Warn("Trying to logout from not authenticated account");
            throw new InvalidOperationException("Trying to logout from not authenticated account");
        }
    }

    public async Task<bool> EnterPincode(string pincode)
    {
        var pincodeRequest = new LoginEmployeeRequest(pincode, DateTime.UtcNow);

        var terminalApi = Locator.Current.GetRequiredService<ITerminalApi>();

        var reponse = await terminalApi.EnterPincode(pincodeRequest);

        if (reponse.IsSuccessStatusCode)
        {
            var pincodeResponse = reponse.Content!;

            var config = Locator.Current.GetRequiredService<IConfiguration>();
            config["MemberAuthToken"] = pincodeResponse.Token;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(pincodeResponse.Token);

            var employeeId = token.Claims.First(claim => claim.Type == "employee_id").Value;

            var memberService = await Locator.Current.GetInitializedService<IMemberService>();

            var member = await memberService.GetMember(Guid.Parse(employeeId));

            var jwtContext = Unsafe.As<JwtAuthenticationContext>(_currentAuthenticationContext.Value);

            jwtContext.Member = member;

            _currentAuthenticationContext.OnNext(jwtContext);

            return true;
        }

        return false;
    }
}
