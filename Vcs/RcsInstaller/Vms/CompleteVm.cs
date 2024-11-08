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
public sealed class CompleteVm : PageVm
{
    public CompleteVm(AbsolutePath path)
    {
        CloseCommand = ReactiveCommand.Create(() =>
        {
            App.Exit();
        });

        StartAppCommand = ReactiveCommand.Create(() =>
        {
            var config = Locator.Current.GetRequiredService<IConfiguration>();

            var app = config["RcsBinName"]!;
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
