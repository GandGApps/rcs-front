using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public class UserDto : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string? Name
    {
        get; set;
    }
}
