using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Avalonia.MarkupExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Avalonia;
internal sealed class BreakpointNotifier: ReactiveObject
{
    public static readonly BreakpointNotifier Instance  = new();

    private BreakpointNotifier()
    {
        this.WhenAnyValue(x => x.Width)
            .Select(AdaptiveMarkupExtension.GetBreakpoint)
            .ToPropertyEx(this, x => x.Breakpoint);
    }

    [Reactive]
    public double Width
    {
        get; set;
    }

    public extern AdaptiveBreakpoint Breakpoint
    {
        [ObservableAsProperty]
        get;
    }
}
