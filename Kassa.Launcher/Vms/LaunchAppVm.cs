using ReactiveUI;
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
        LaunchAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var path = Environment.GetEnvironmentVariable("KASSA_INSTALL_PATH", EnvironmentVariableTarget.User);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException("KASSA_INSTALL_PATH is not set.");
            }

#if _WINDOWS
            path = Path.Combine(path, "Kassa.Wpf.exe");
#else
            path = Path.Combine(path, "Kassa");
#endif
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

    }

    public ReactiveCommand<Unit, Unit> LaunchAppCommand
    {
        get;
    }

}
