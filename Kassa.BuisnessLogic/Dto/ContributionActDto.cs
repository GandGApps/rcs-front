using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public class ContributionActDto: IGuidId
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

    public Guid ContributionReasonId
    {
        get; set;
    }

}