using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using RcsInstaller.Vms;

namespace RcsInstaller.Pages;

public partial class WelcomePage : ReactiveUserControl<WelcomeVm>
{
    public WelcomePage()
    {
        InitializeComponent();
    }
}