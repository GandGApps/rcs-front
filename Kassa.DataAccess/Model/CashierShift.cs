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

    public Guid[] OpennedOrderIds
    {
        get; set;
    }

    public Guid[] ClosedOrderIds
    {
        get; set;
    }

    public Guid[] ClosedShiftClosedOrderIds
    {
        get; set;
    }
}
