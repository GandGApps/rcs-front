using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kassa.Wpf;
public static class Program
{

    [STAThread]
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging();
        builder.Services.AddRcsLoggers(RcsKassa.LogsPath);

        var app = builder.Build();


        if (args.Contains("--no-launcher", StringComparer.InvariantCultureIgnoreCase))
        {
            App.Main();
        }
        else
        {
            LaunchLauncher();
        }
    }

    private static void LaunchLauncher()
    {
        var launcherPath = Environment.GetEnvironmentVariable("KASSA_LAUNCHER_PATH", EnvironmentVariableTarget.Machine);

        if (string.IsNullOrWhiteSpace(launcherPath))
        {
            throw new InvalidOperationException("KASSA_LAUNCHER_PATH environment variable is not set.");
        }

        var path = System.IO.Path.Combine(launcherPath, "Kassa.Launcher.exe");

        Process.Start(path);
    }

}
