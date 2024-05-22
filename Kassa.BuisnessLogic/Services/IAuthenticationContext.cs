using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IAuthenticationContext
{
    public string Token
    {
        get;
    }

    /// <summary>
    /// It's terminal user, not member
    /// </summary>
    public UserDto? User
    {
        get;
    }

    public MemberDto? Member
    {
        get;
    }

    public bool IsAuthenticated
    {
        get;
    }
}
