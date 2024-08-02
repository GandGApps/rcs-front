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
        var directories = Directory.GetDirectories(path);

        var i = 0d;
        var totalFiles = directories.Length + files.Length;

        foreach (var file in files)
        {
            progress(i++ / totalFiles);
            File.Delete(file);
            this.Log().Info($"Deleted file: {file}");
        }

        foreach (var directory in directories)
        {
            progress(i++ / totalFiles);
            Directory.Delete(directory, true);
            this.Log().Info($"Deleted directory: {directory}");
        }

        RemoveRegisty();
    }

    private void RemoveRegisty()
    {
        Registry.LocalMachine.DeleteSubKey(LauncherConstants.RegistryKeyPath, false);

        this.Log().Info("Deleted registry key.");
    }
}
