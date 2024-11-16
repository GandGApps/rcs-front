using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RcsInstaller.Progress;
using TruePath;

namespace RcsInstaller.Services;
public interface IInstaller
{
    public Task InstallAsync(AbsolutePath path, bool createShortcut, Action<ProgressState> progress) => InstallAsync(path, HelperExtensions.EmptyVersion, createShortcut, progress);

    /// <summary>
    /// Installs the application asynchronously, allowing for optional shortcut creation.
    /// </summary>
    /// <param name="path">The file path to install the application to.</param>
    /// <param name="currentVersion">The current application version. If the application is not installed, the current version should be set to <see cref="HelperExtensions.EmptyVersion"/>.</param>
    /// <param name="createShortcut">Indicates whether a shortcut should be created.</param>
    /// <param name="progress">The action to report installation progress.</param>
    public Task InstallAsync(AbsolutePath path, Version currentVersion, bool createShortcut, Action<ProgressState> progress);
}
