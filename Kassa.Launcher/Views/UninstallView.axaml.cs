using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Kassa.Launcher.Vms;
using Kassa.Launcher.Vms;
using System;
using ReactiveUI;

namespace Kassa.Launcher;

public partial class UninstallView : ReactiveUserControl<UninstallVm>
{
    public UninstallView()
    {
        AvaloniaXamlLoader.Load(this);

        this.WhenActivated(disposables =>
        {
            ViewModel.RemoveCommand.Execute().Subscribe();
        });
    }
}