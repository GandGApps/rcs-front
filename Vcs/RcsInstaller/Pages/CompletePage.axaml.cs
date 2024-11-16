using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using RcsInstaller.Vms;
using ReactiveUI;

namespace RcsInstaller;

public sealed partial class CompletePage : ReactiveUserControl<CompletePageVm>
{
    public CompletePage()
    {
        InitializeComponent();
    }

    
}