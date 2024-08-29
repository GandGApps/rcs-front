using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Kassa.RxUI;
using Kassa.Shared.ServiceLocator;

namespace Kassa.Avalonia;
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        Dispatcher.UIThread.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
        {
            var vm = RcsLocator.GetRequiredService<MainViewModel>();
            e.Handled = true;
        };

        Dispatcher.UIThread.UnhandledExceptionFilter += (object sender, DispatcherUnhandledExceptionFilterEventArgs e) =>
        {

        };
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}