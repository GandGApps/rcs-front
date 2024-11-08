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
using CommunityToolkit.Diagnostics;

namespace RcsInstaller.Services;
public sealed class WndShortcutCreator : IShortcutCreator
{
    public Task CreateSrotcut(AbsolutePath path, string name)
    {
        var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.lnk");
        
        var directoryPath = Directory.Exists(path.Value) ? path.Value : Path.GetDirectoryName(name);

        if (directoryPath is null)
        {
            ThrowHelper.ThrowArgumentException(nameof(path), $"The provided path '{path.Value}' is invalid. It must be a valid directory path or lead to an existing file.");
        }

        var shell = new WshShell();

        var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
        shortcut.IconLocation = Path.Combine(directoryPath, "Logo.ico");
        shortcut.TargetPath = path.Value;

        shortcut.Save();

        return Task.CompletedTask;
    }
}
