using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IAuthenticationContext
{
    public UserDto? User
    {
        get;
    }

    public bool IsAuthenticated
    {
        get;
    }
}
