using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kassa.Wpf;

public sealed class HostedWpf : IHostedService
{

    [STAThread]
    [DebuggerNonUserCode]
    public Task StartAsync(CancellationToken cancellationToken)
    {
        App.Main();


        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        App.Current.Shutdown();

        return Task.CompletedTask;
    }
}
