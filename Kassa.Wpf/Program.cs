using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Splat;

namespace Kassa.Wpf;
public static class Program
{

    [STAThread]
    public static void Main(string[] args)
    {
        

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

        if (!args.Contains("--no-launcher", StringComparer.InvariantCultureIgnoreCase))
        {
            LaunchLauncher(app.Services.GetRequiredService<ILogger<App>>());
            return;
        }

        app.Run();
    }

    private static void LaunchLauncher(ILogger<App> logger)
    {
        var launcherPath = Path.Combine(RcsKassa.BasePath, "Launcher");
        var path = System.IO.Path.Combine(launcherPath, "Kassa.Launcher.exe");
        var parameters = $"-p {RcsKassa.BasePath}";

        logger.LogInformation("Start proccess:{path} {parameters}", path, parameters);

        Process.Start(path, parameters);
    }

}
