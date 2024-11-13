using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kassa.Wpf;

public sealed class HostedWpf(ILogger<HostedWpf> logger, IRcsvcApi rcsvcApi) : IHostedService
{
    private readonly ILogger<HostedWpf> _logger = logger;

    [STAThread]
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var latestVersion = rcsvcApi.GetVersion().GetAwaiter().GetResult();

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        if(currentVersion < latestVersion)
        {
            _logger.LogInformation("Detected new Version. Current is {currentVersion}, latest is {latestVersion}", currentVersion, latestVersion);
            LaunchUpdates(currentVersion);
            return;
        }

        var args = Environment.GetCommandLineArgs();

        if (!args.Contains("--no-launcher"))
        {
            LaunchLauncher();
            await RcsKassa.Host.StopAsync(default);
            return;
        }

        App.Main();

        await RcsKassa.Host.StopAsync(default);
    }

    private void LaunchLauncher()
    {
        var launcherPath = Path.Combine(RcsKassa.BasePath, "Launcher");
        var path = System.IO.Path.Combine(launcherPath, "Kassa.Launcher.exe");
        var parameters = $"-p {RcsKassa.BasePath}";

        _logger.LogInformation("Start proccess:{path} {parameters}", path, parameters);

        Process.Start(path, parameters);
    }

    private void LaunchUpdates(Version version)
    {
        var launcherPath = Path.Combine(RcsKassa.BasePath, "Vcs");
        var path = System.IO.Path.Combine(launcherPath, "RcsInstaller.exe");
        var parameters = $"-p {RcsKassa.BasePath} -v {version}";

        _logger.LogInformation("Start proccess:{path} {parameters}", path, parameters);

        Process.Start(path, parameters);
    }

    [STAThread]
    [DebuggerNonUserCode]
    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (App.Current is null)
        {
            _logger.LogDebug($"{nameof(App.Current)} is null. This might happen if StopAsync is called twice.");
            return Task.CompletedTask;
        }

        App.Current.Shutdown();

        return Task.CompletedTask;
    }
}
