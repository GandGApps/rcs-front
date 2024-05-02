using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
internal class AuthService : IAuthService
{
    private static readonly UserDto _admin = new()
    {
        Id = Guid.NewGuid(),
        Name = "Admin"
    };

    private readonly BehaviorSubject<IAuthenticationContext> _currentAuthenticationContext = new(AuthenticationContext.NotAuthenticated);
    private readonly ObservableOnlyBehaviourSubject<IAuthenticationContext> _observableOnly;

    public AuthService()
    {
        _observableOnly = new(_currentAuthenticationContext);
    }

    public IObservableOnlyBehaviourSubject<IAuthenticationContext> CurrentAuthenticationContext => _observableOnly;

    public bool IsAuthenticated => _currentAuthenticationContext.Value.IsAuthenticated;

    public Task<bool> AuthenticateAsync(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            _currentAuthenticationContext.OnNext(new AuthenticationContext
            {
                User = _admin
            });
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
    public Task<bool> LogoutAsync()
    {
        _currentAuthenticationContext.OnNext(AuthenticationContext.NotAuthenticated);
        return Task.FromResult(true);
    }

    public async Task<bool> IsManagerPincode(string pincode)
    {
        await Task.Delay(2000);

        return pincode == "1234";
    }

    private class AuthenticationContext : IAuthenticationContext
    {
        public static readonly AuthenticationContext AdminAuthenticated = new()
        {
            User = _admin
        };

        public static readonly AuthenticationContext NotAuthenticated = new()
        {
            User = null
        };

        public UserDto? User
        {
            get; set;
        }

        public string Token
        {
            get;
        }

        public bool IsAuthenticated => User is not null;
    }
}
