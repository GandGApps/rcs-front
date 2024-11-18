using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RcsInstaller.Progress;
using TruePath;

namespace RcsInstaller.Services;
public interface IUpdater
{
    /// <summary>
    /// Updates the application to a specified version asynchronously.
    /// </summary>
    /// <param name="path">The file path to update the application.</param>
    /// <param name="currentVersion">The current application version. If the application is not installed, the current version should be set to <see cref="HelperExtensions.EmptyVersion"/>.</param>
    /// <param name="callback">The action to report update progress.</param>
    public Task UpdateAsync(Action<ProgressState> callback);
}
