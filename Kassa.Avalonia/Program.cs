using System;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Serilog;
using Splat;

namespace Kassa.Avalonia;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            LogHost.Default.Fatal((Exception)e.ExceptionObject, "An unhandled exception occurred");
        };

        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
        {
            LogHost.Default.Error(ex, "An unhandled exception occurred");
        });

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            e.Exception.Handle(ex =>
            {
                LogHost.Default.Error(ex, "An unobserved exception occurred");
                return true;
            });
            e.SetObserved();
        };

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogHost.Default.Fatal(e, "An unhandled exception occurred");
            return;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
}