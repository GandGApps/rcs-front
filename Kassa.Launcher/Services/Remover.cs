using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Splat;

namespace Kassa.Launcher.Services;
internal sealed class Remover : IRemover, IEnableLogger
{
    public async Task RemoveAsync(Action<double> progress)
    {
        var pathManager = Locator.Current.GetService<IApplicationPathManager>()!;
        var path = await pathManager.GetApplicationPath();

        var files = Directory.GetFiles(path);

        var i = 0;
        foreach (var file in files)
        {
            progress(i++ / files.Length);
            File.Delete(file);
            this.Log().Info($"Deleted file: {file}");
        }

        RemoveRegisty();
    }

    private void RemoveRegisty()
    {
        Registry.LocalMachine.DeleteSubKey(LauncherConstants.RegistryKeyPath, false);

        this.Log().Info("Deleted registry key.");
    }
}
