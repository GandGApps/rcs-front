using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Kassa.Launcher.Vms;
using KassaLauncher.Vms;
using System;
using ReactiveUI;

namespace KassaLauncher;

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