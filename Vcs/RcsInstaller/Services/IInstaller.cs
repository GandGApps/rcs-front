using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Services;
public interface IInstaller
{
    public Task InstallAsync(string path, bool createShortcut, Action<ProgressState> progress) => InstallAsync(path, HelperExtensions.EmptyVersion, createShortcut, progress);
    public Task InstallAsync(string path, Version version, bool createShortcut, Action<ProgressState> progress);
}
