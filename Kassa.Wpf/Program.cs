﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Edgar;
using Kassa.BuisnessLogic.Services;
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

        builder.Services.AddRcsvcApi();

        builder.Services.AddLogging();
        builder.Services.AddRcsLoggers(RcsKassa.LogsPath);
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

        builder.Services.AddScopedInitializablesFromServiceCollection();

        builder.Logging.AddSerilog();

        builder.Configuration.AddRcsConfiguration();

        builder.Services.AddHostedWpf();

        var app = builder.Build();

        RcsKassa.Host = app;

        app.Run();
    }

    

}
