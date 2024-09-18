using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using Kassa.Launcher;
using Kassa.Launcher.Services;
using KassaLauncher.Vms;
using Splat;

namespace KassaLauncher;

public partial class LaunchApp : ReactiveUserControl<LaunchAppVm>
{
    public LaunchApp()
    {
        InitializeComponent();
    }

    private void OpenSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = new SettingsWindow();

        var optionsManager = Locator.Current.GetService<IOptionManager>()!;

        window.ViewModel.SelectedPrinter = window.ViewModel.Printers.FirstOrDefault(p => p.Name == optionsManager.GetOption<string>("PrinterName"), window.ViewModel.Printers[0]);
        window.ViewModel.OposCashDrawer.IsSelected = optionsManager.GetOption<string>("CashDrawerPosLib") == "WndPosLib";
        window.ViewModel.EscPosUsbCashDrawer.IsSelected = optionsManager.GetOption<string>("CashDrawerPosLib") == "EscposUsb";
        window.ViewModel.IsOposOrKeyboardCardReader = optionsManager.GetOption<string>("MsrReaderLib") == "WndPosLib";
        window.ViewModel.SuffixCardReaderKeyboard = optionsManager.GetOption<string>("MsrReaderLibKeyboardSuffix");
        window.ViewModel.PrefixCardReaderKeyboard = optionsManager.GetOption<string>("MsrReaderLibKeyboardPrefix");

        var root = this.GetVisualRoot();

        window.Show((Window)root!);
    }
}