using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public class CourierDto: IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string FirstName
    {
        get; set;
    }

    public string LastName
    {
        get; set;
    }

    public string MiddleName
    {
        get; set;
    }

    public string Phone
    {
        get; set;
    }
}
