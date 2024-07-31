using Kassa.Launcher.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Vms;

public sealed class LaunchAppVm : BaseVm
{

    public LaunchAppVm()
    {
        var pathManager = Locator.Current.GetService<IApplicationPathManager>()!;
        var remover = Locator.Current.GetService<IRemover>()!;

        LaunchAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var path = await pathManager.GetApplicationPath();

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException("KASSA_INSTALL_PATH is not set.");
            }

            path = Path.Combine(path, "Kassa.Wpf.exe");

            if (!File.Exists(path))
            {
                throw new InvalidOperationException("Kassa is not installed.");
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                }
            };

            process.Start();

            await process.WaitForExitAsync();
        });

        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await remover.RemoveAsync();
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
