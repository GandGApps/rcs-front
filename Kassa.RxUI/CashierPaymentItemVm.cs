using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class CashierPaymentItemVm : ReactiveObject
{

    public CashierPaymentItemVm()
    {
        this.WhenAnyValue(x => x.Entered)
            .Select(x => x > 0)
            .ToPropertyEx(this, x => x.IsEntered);

        RemoveItemCommand = ReactiveCommand.Create(() =>
        {
            Entered = 0;
        });
    }

    [Reactive]
    public double Entered
    {
        get; set;
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = null!;

    [Reactive]
    public string Description
    {
        get; set;
    } = null!;

    public ReactiveCommand<Unit, Unit> RemoveItemCommand
    {
        get;
    }

    public extern bool IsEntered
    {
        [ObservableAsProperty]
        get;
    }
}
