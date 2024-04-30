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
