using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class ShiftRowViewModel : ReactiveObject
{
    [Reactive]
    public int Number
    {
        get; set;
    }

    [Reactive]
    public string? Name
    {
        get; set;
    }

    [Reactive]
    public string Begin
    {
        get; set;
    }

    [Reactive]
    public string End
    {
        get; set;
    }

    [Reactive]
    public string Break
    {
        get; set;
    }

    [Reactive]
    public double HourlyRate
    {
        get; set;
    }

    [Reactive]
    public double Earned
    {
        get; set;
    }

    [Reactive]
    public double Fine
    {
        get; set;
    }

    [Reactive]
    public string Comment
    {
        get; set;
    }

    [Reactive]
    public string Manager
    {
        get; set;
    }
}
