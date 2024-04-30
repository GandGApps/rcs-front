using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class WithdrawalReason : IModel
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }
}
