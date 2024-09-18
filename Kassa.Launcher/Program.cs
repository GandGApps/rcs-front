using Avalonia;
using Avalonia.ReactiveUI;
using Kassa.Launcher.Services;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using KassaLauncher.Services;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Serilog;
using Splat;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

namespace KassaLauncher;

internal sealed class Program
{

    private static  IConfiguration _configuration = null!;

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

        Environment.SetEnvironmentVariable("KASSA_LAUNCHER_PATH", App.BasePath, EnvironmentVariableTarget.Machine);

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();

        var repoInfo = _configuration.GetRequiredSection("RepoInfo").Get<RepoInfo>();

        Debug.Assert(repoInfo is not null);

        var pathManager = new EnvironmentPathManager();
        var githubUpdater = new GitHubUpdater(repoInfo, new WndShortcutCreator(), pathManager);

        Locator.CurrentMutable.RegisterConstant(githubUpdater, typeof(IUpdater));
        Locator.CurrentMutable.RegisterConstant(githubUpdater, typeof(IInstaller));
        Locator.CurrentMutable.RegisterConstant(new JsonAppsettingsSaver(), typeof(IOptionManager));
        Locator.CurrentMutable.RegisterConstant(pathManager, typeof(IApplicationPathManager));
        Locator.CurrentMutable.RegisterConstant(new Remover(), typeof(IRemover));
        Locator.CurrentMutable.RegisterConstant(new SelfProccesUpdater(repoInfo), typeof(ISelfUpdater));

        SharedServices.AddLoggers();

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
