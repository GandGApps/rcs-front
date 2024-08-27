using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Launcher.Vms;
public sealed class CashDrawerVm : ReactiveObject
{
    public static readonly CashDrawerVm EscPosUsbCashDrawer = new();
    public static readonly CashDrawerVm OposCashDrawer = new();

    [Reactive]
    public bool IsSelected
    {
        get; set;
    }
}
