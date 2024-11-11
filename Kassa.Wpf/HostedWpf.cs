using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kassa.Wpf;

public sealed class HostedWpf(ILogger<HostedWpf> logger) : IHostedService
{
    private readonly ILogger<HostedWpf> _logger = logger;

    [STAThread]
    [DebuggerNonUserCode]
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        App.Main();

        await RcsKassa.Host.StopAsync(default);
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
