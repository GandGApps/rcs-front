using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Kassa.Launcher.Vms;

namespace Kassa.Launcher;

public sealed partial class InitView : ReactiveUserControl<InitVm>
{
    public InitView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}