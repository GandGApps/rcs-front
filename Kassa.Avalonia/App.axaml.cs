using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Kassa.RxUI;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;

namespace Kassa.Avalonia;
public sealed partial class App : Application
{
    public static new App Current => Unsafe.As<App>(Application.Current!);

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        Dispatcher.UIThread.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
        {

            if (e.Exception is FatalException)
            {
                return;
            }

            var vm = RcsLocator.GetRequiredService<MainViewModel>();

            e.Handled = vm.TryHandleUnhandled(Dispatcher.UIThread, e.Exception);
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