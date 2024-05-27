using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class CashierShift : IModel
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
