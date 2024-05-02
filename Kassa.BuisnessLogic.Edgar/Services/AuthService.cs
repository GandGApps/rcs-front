using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal class AuthService : IAuthService
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

    private class JwtAuthenticationContext : IAuthenticationContext
    {
        public static readonly JwtAuthenticationContext NotAuthenticated = new()
        {
            User = null
        };

        public UserDto? User
        {
            get; set;
        }

        public string Token
        {
            get; set;
        } = string.Empty;

        public bool IsAuthenticated => User is not null || !string.IsNullOrWhiteSpace(Token);
    }
}
