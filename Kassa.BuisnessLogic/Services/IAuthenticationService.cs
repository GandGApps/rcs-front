using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;
public interface IAuthenticationService
{
    public IObservable<IAuthenticationContext> CurrentAuthenticationContext
    {
        get;
    }

    public bool IsAuthenticated
    {
        get;
    }

    public Task<bool> AuthenticateAsync(string username, string password);
    public Task<bool> LogoutAsync();
}
