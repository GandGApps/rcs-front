using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
public class ClientDto
{
    public Guid Id
    {
        get; set;
    }

    public string FullName
    {
        get; set;
    }

    public string Address
    {
        get; set;
    }

    public string Phone
    {
        get; set;
    }
}
