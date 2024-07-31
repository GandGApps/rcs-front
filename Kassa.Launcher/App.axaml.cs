using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kassa.Launcher.Services;
using KassaLauncher.Services;
using Microsoft.Extensions.Configuration;
using Splat;

namespace KassaLauncher;

public partial class App : Application
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

        var githubUpdater = new GitHubUpdater(repoInfo, new WndShortcutCreator());

        Locator.CurrentMutable.RegisterConstant(githubUpdater, typeof(IUpdater));
        Locator.CurrentMutable.RegisterConstant(githubUpdater, typeof(IInstaller));
        Locator.CurrentMutable.RegisterConstant(new JsonAppsettingsSaver(), typeof(IOptionManager));
        Locator.CurrentMutable.RegisterConstant(new EnvironmentPathManager(), typeof(IApplicationPathManager));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

            if(desktop.Args?.Length > 0 && desktop.Args.Contains("--remove"))
            {

            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}