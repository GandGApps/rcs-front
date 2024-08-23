using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IAuthService
{
    public IObservableOnlyBehaviourSubject<IAuthenticationContext> CurrentAuthenticationContext
    {
        get;
    }

    public bool IsAuthenticated
    {
        get;
    }

    public Task<bool> EnterPincode(string pincode);
    public Task<bool> AuthenticateAsync(string username, string password);
    public Task<bool> LogoutAsync();
    public Task<bool> IsManagerPincode(string pincode);
    public Task<bool> CheckPincode(MemberDto member, string pincode);
    public ValueTask<bool> LogoutAccount();
}
