
using CommunityToolkit.Diagnostics;
using RcsInstaller.Pages;
using RcsInstaller.Vms;
using ReactiveUI;
using System;

namespace RcsInstaller;

public sealed class ViewLocator : IViewLocator
{
    // Maybe this should be a dictionary?
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
    {
        WelcomePageVm context => new WelcomePage { DataContext = context },
        SelectPathPageVm context => new SelectPathPage { DataContext = context },
        InstallingPageVm context => new InstallingPage { DataContext = context },
        CompletePageVm context => new CompletePage { DataContext = context },
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IViewFor>(nameof(viewModel))
    };
}
