using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Edgar;
using Kassa.DataAccess;
using Kassa.DataAccess.HttpRepository;
using Kassa.RxUI;
using Kassa.Shared;
using Kassa.Wpf.Services.CashDrawers;
using Kassa.Wpf.Services.MagneticStripeReaders;
using Kassa.Wpf.Services.PosPrinters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Serilog;
using Splat;

namespace Kassa.Wpf;
public static class Program
{

    [STAThread]
    public static void Main(string[] args)
    {
        /*if (!args.Contains("--no-launcher", StringComparer.InvariantCultureIgnoreCase))
        {
            LaunchLauncher();
            return;
        }*/

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging();
        builder.Services.AddRcsLoggers(RcsKassa.LogsPath);
        builder.Services.AddHostedService<HostedWpf>();
        builder.Services.AddDispatherAdapter();

        builder.Services.AddPrinterPosLib(builder.Configuration);
        builder.Services.AddMsrReaderPosLib(builder.Configuration);
        builder.Services.AddCashDrawerPosLib(builder.Configuration);

        Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.Wpf);

        builder.Services.AddMockBuisnessLogic();
        builder.Services.AddMockDataAccess();

        builder.Services.AddEdgarBuisnessLogic();
        builder.Services.AddHttpRepositories();

        builder.Services.AddMainViewModelProvider();

        builder.Logging.AddSerilog();

        builder.Configuration.AddRcsConfiguration();



        var app = builder.Build();

        RcsKassa.Host = app;

        app.Run();
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
