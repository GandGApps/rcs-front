using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using RcsInstaller.Vms;

namespace RcsInstaller.Pages;

public sealed partial class WelcomePage : ReactiveUserControl<WelcomePageVm>
{
    public WelcomePage()
    {
        InitializeComponent();
    }
}