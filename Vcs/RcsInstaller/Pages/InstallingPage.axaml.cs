using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using RcsInstaller.Vms;

namespace RcsInstaller;

public sealed partial class InstallingPage : ReactiveUserControl<InstallingPageVm>
{
    public InstallingPage()
    {
        InitializeComponent();
    }
}