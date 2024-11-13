using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruePath;

namespace RcsInstaller.Services;
public interface IInstaller
{
    public Task InstallAsync(AbsolutePath path, bool createShortcut, Action<ProgressState> progress) => InstallAsync(path, HelperExtensions.EmptyVersion, createShortcut, progress);
    public Task InstallAsync(AbsolutePath path, Version version, bool createShortcut, Action<ProgressState> progress);
    public Task UpdateAsync(AbsolutePath path, Version version, Action<ProgressState> value);
}
