using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

namespace Kassa.Launcher.Services;
internal sealed class EnvironmentPathManager: IApplicationPathManager
{

    public Task<string> GetApplicationPath()
    {
        var path = Environment.GetEnvironmentVariable(LauncherConstants.ApplicationPathKey, EnvironmentVariableTarget.Machine);

        if (string.IsNullOrWhiteSpace(path))
        {
            ThrowHelper.ThrowInvalidOperationException("Application path is not set.");
        }

        return Task.FromResult(path);
    }

    public Task SetApplicationPath(string path)
    {
        Environment.SetEnvironmentVariable(LauncherConstants.ApplicationPathKey, path, EnvironmentVariableTarget.Machine);

        return Task.CompletedTask;
    }
}