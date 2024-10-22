using Avalonia;
using Avalonia.ReactiveUI;
using Kassa.Launcher.Services;
using Kassa.Shared;
using Microsoft.Extensions.Configuration;
using RcsInstaller;
using ReactiveUI;
using Serilog;
using Splat;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Threading.Tasks;

namespace Kassa.Launcher;

internal sealed class Program
{

    private static IConfiguration _configuration = null!;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KassaLauncherOption))]
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

        Environment.SetEnvironmentVariable("KASSA_LAUNCHER_PATH", App.BasePath, EnvironmentVariableTarget.Machine);

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();

        var pathManager = new PathConstantMaanager(args[0]);

        Locator.CurrentMutable.RegisterConstant(new JsonAppsettingsSaver(), typeof(IOptionManager));
        Locator.CurrentMutable.RegisterConstant(pathManager, typeof(IApplicationPathAccessor));

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            LogHost.Default.Fatal(exception, "Something very bad happened");
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
            .UseReactiveUI()
            .WithInterFont()
            .LogToTrace();
}
