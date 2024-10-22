using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;
public interface IInstaller
{
    public Task InstallAsync(string path, bool createShortcut, Action<double> progress);
}
