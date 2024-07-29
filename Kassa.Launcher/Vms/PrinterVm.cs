using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Launcher.Vms;
public sealed class PrinterVm : ReactiveObject
{
    public static readonly PrinterVm Empty = new("Без принтера");

    public PrinterVm(string name)
    {
        Name = name;
    }

    [Reactive]
    public string Name
    {
        get; set;
    }

    [Reactive]
    public bool IsSelected
    {
        get; set;
    }
}