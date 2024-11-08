using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TruePath;

namespace RcsInstaller.Services;
public sealed class WndShortcutCreator : IShortcutCreator
{
    public Task CreateSrotcut(AbsolutePath path, string name)
    {
        var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.lnk");
        var directoryPath = Path.GetDirectoryName(path.Value);

        Debug.Assert(directoryPath is not null);

        var shell = new WshShell();

        var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
        shortcut.IconLocation = Path.Combine(directoryPath, "Logo.ico");
        shortcut.TargetPath = path.Value;

        shortcut.Save();

        return Task.CompletedTask;
    }
}
