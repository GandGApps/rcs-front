using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public class DepositActDto: IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public double Amount
    {
        get; set;
    }

    public DateTime Date
    {
        get; set;
    }

    public Guid? MemberId
    {
        get; set;
    }

    public Guid DepositReasonId
    {
        get; set;
    }

}