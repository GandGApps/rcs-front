using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TruePath;

namespace RcsInstaller.Vms;
public sealed class CompletePageVm : PageVm
{
    public CompletePageVm(AbsolutePath path, IConfiguration configuration)
    {
        CloseCommand = ReactiveCommand.Create(() =>
        {
            App.Exit();
        });

        StartAppCommand = ReactiveCommand.Create(() =>
        {
            var app = configuration["RcsBinName"]!;
            var appPath = path / app;

            Process.Start(appPath.Value);

            App.Exit();
        });
    }


    public ReactiveCommand<Unit, Unit> CloseCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> StartAppCommand
    {
        get;
    }
}
