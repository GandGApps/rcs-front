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

namespace RcsInstaller.Vms;
public sealed class CompleteVm : PageVm
{
    public CompleteVm(string path)
    {
        CloseCommand = ReactiveCommand.Create(() =>
        {
            App.Exit();
        });

        StartAppCommand = ReactiveCommand.Create(() =>
        {
            var config = Locator.Current.GetRequiredService<IConfiguration>();

            var app = config["RcsBinName"]!;
            var appPath = System.IO.Path.Combine(path, app);

            Process.Start(appPath);

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
