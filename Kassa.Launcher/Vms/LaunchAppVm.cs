﻿using CommunityToolkit.Diagnostics;
using Kassa.Launcher.Services;
using Kassa.Launcher.Vms;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Vms;

public sealed class LaunchAppVm : BaseVm
{

    public LaunchAppVm()
    {
        var pathManager = Locator.Current.GetService<IApplicationPathAccessor>()!;
        var remover = Locator.Current.GetService<IRemover>()!;

        LaunchAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var path = pathManager.GetApplicationPath();

            if (string.IsNullOrWhiteSpace(path))
            {
                ThrowHelper.ThrowInvalidOperationException("Kassa is not installed.");
            }

            LogHost.Default.Info($"App folder path is {path}");

            var appPath = Path.Combine(path, "Kassa.Wpf.exe");

            LogHost.Default.Info($"App path is {appPath}");

            if (!File.Exists(appPath))
            {
                ThrowHelper.ThrowInvalidOperationException("Kassa is not installed.");
            }

            LogHost.Default.Info($"Launching Kassa from {appPath}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = true,
                    ArgumentList = { "--no-launcher" },
                    WorkingDirectory = path,
                }
            };

            process.Start();

            App.Exit();
        });

        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var remover = Locator.Current.GetService<IRemover>()!;
            var uninstallVm = new UninstallVm(remover);

            HostScreen.Router.NavigateAndReset.Execute(uninstallVm).Subscribe();
        });
    }

    public ReactiveCommand<Unit, Unit> LaunchAppCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

}
