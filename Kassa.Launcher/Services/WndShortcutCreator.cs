using Shellify;
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
    public Task CreateShortcutAsync(string path, string name)
    {
        var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + name + ".lnk";
        var targetPath = path;

        var shellLink = ShellLinkFile.CreateAbsolute(targetPath);
        var directoryTargetPath = Path.GetDirectoryName(targetPath);

        Debug.Assert(directoryTargetPath is not null);

        shellLink.IconLocation = Path.Combine(directoryTargetPath, "Logo.ico");

        shellLink.SaveAs("./link.lnk");

        return Task.CompletedTask;
    }
}

#endif