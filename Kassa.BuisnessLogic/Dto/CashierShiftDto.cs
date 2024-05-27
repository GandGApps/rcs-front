using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public class CashierShiftDto : IModel
{
    public Guid Id
    {
        get; set;
    }

    public int Number
    {
        get; set;
    }

    public Guid MemberId
    {
        get; set;
    }

    public DateTime? Start
    {
        get; set;
    }

    public DateTime? End
    {
        get; set;
    }

    public string? Note
    {
        get; set;
    }

    public bool IsStarted
    {
        get; set;
    }

    public Guid ManagerId
    {
        get; set;
    }

    public Guid[] OrderIds
    {
        get; set;
    } = [];
}
