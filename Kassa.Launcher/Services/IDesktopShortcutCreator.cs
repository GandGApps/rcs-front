using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;

internal interface IDesktopShortcutCreator
{
    public Task CreateShortcutAsync(string path, string name);
}
