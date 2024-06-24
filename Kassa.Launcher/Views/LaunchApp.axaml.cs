using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using KassaLauncher.Vms;

namespace KassaLauncher;

public partial class LaunchApp : ReactiveUserControl<LaunchAppVm>
{
    public LaunchApp()
    {
        InitializeComponent();
    }
}