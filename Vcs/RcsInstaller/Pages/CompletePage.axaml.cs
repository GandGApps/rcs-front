using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using RcsInstaller.Vms;

namespace RcsInstaller;

public sealed partial class CompletePage : ReactiveUserControl<CompleteVm>
{
    public CompletePage()
    {
        InitializeComponent();
    }
}