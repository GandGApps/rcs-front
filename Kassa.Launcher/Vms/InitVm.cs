using Avalonia.Threading;
using Kassa.Launcher.Services;
using Kassa.Launcher.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Vms;

public sealed class InitVm : BaseVm
{
    private readonly IApplicationPathAccessor _pathManager;

    [Reactive]
    public bool IsInstalled
    {
        get; private set;
    }

    [Reactive]
    public double Progress
    {
        get; private set;
    }

    [Reactive]
    public string? Status
    {
        get; private set;
    }

    public InitVm(IApplicationPathAccessor pathManager)
    {
        _pathManager = pathManager;
    }

    public async Task InitAsync()
    {
        var installedPath = await _pathManager.GetApplicationPath();

        if (string.IsNullOrEmpty(installedPath))
        {
            Dispatcher.UIThread.Invoke(() => IsInstalled = false);
            
        }
    }
}
