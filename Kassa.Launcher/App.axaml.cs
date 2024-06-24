using System;
using System.Diagnostics;
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

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();

        var repoInfo = _configuration.GetRequiredSection("RepoInfo").Get<RepoInfo>();

        Debug.Assert(repoInfo is not null);

        Locator.CurrentMutable.RegisterConstant(new GitHubUpdater(repoInfo, new WndShortcutCreator()), typeof(IUpdater));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        base.OnFrameworkInitializationCompleted();
    }
}