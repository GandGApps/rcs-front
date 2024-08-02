using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kassa.Launcher.Services;
using Kassa.Launcher.Vms;
using Kassa.Shared;
using KassaLauncher.Services;
using KassaLauncher.Vms;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;

namespace KassaLauncher;

public partial class App : Application, IEnableLogger
{
    private IConfiguration _configuration = null!;

    public IConfiguration Configuration => _configuration;

    public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

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

        Locator.CurrentMutable.AddLoggers();

        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
        {
            this.Log().Error(ex, "An unhandled exception occurred");
        });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var mainVm = (MainVm)desktop.MainWindow.DataContext!;

            if (desktop.Args?.Length > 0 && desktop.Args.Contains("--remove"))
            {
                var remover = Locator.Current.GetService<IRemover>()!;

                mainVm.Router.NavigateAndReset.Execute(new UninstallVm(remover)).Subscribe();
            }
            else
            {
                mainVm.Start.Execute().Subscribe();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}