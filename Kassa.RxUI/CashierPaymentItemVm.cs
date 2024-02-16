using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class CashierPaymentItemVm : ReactiveObject
{
    [Reactive]
    public double Cost
    {
        get; set;
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    }

    [Reactive]  
    public string Name
    {
        get; set;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> RemoveItem
    {
        get; set;
    }
}
