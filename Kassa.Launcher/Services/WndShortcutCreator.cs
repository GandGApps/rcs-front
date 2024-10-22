using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Services;

#if _WINDOWS

internal sealed class WndShortcutCreator : IDesktopShortcutCreator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path to exe</param>
    /// <param name="name">Name for lnk</param>
    /// <returns></returns>
    public Task CreateShortcutAsync(string path, string name)
    {
        var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.lnk");
        var directoryPath = Path.GetDirectoryName(path);

        Debug.Assert(directoryPath is not null);

        var shell = new WshShell();

        var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
        shortcut.IconLocation = Path.Combine(directoryPath, "Logo.ico");
        shortcut.TargetPath = path;

        shortcut.Save();

        return Task.CompletedTask;
    }
}

#endif