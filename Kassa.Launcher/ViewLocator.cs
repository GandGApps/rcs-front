using Kassa.Launcher.Vms;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher;

public sealed class ViewLocator : IViewLocator
{
    // Maybe this should be a dictionary?
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
    {
        InitVm context => new InitView { DataContext = context },
        InstallerVm context => new InstallerView { DataContext = context },
        LaunchAppVm context => new LaunchApp { DataContext = context },
        UninstallVm context => new UninstallView { DataContext = context },
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}
