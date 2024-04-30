using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Kassa.RxUI;
public class ServiceOrderRowViewModel : ReactiveObject
{
    public int Number
    {
        get; set;
    }

    public string Time
    {
        get; set;
    }

    public string CashierName
    {
        get; set;
    }

    public string Amount
    {
        get; set;
    }

    public string Composition
    {
        get; set;
    }

    public string ExternalNumber
    {
        get; set;
    }

    public string ReceiptNumber
    {
        get; set;
    }
}
