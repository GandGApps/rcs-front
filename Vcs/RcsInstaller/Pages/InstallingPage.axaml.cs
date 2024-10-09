using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using RcsInstaller.Vms;

namespace RcsInstaller;

public partial class InstallingPage : ReactiveUserControl<InstallingVm>
{
    public InstallingPage()
    {
        InitializeComponent();
    }
}