using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Options;
using RcsInstaller.Configurations;
using TruePath;

namespace RcsInstaller.Services;
internal sealed class TargetAppRunner : ITargetAppRunner
{
    private readonly IAppRegistry _appRegistry;
    private readonly IOptions<TargetAppInfo> _targetAppInfoOptions;

    public TargetAppRunner(IAppRegistry appRegistry, IOptions<TargetAppInfo> targetAppInfoOptions)
    {
        _appRegistry = appRegistry;
        _targetAppInfoOptions = targetAppInfoOptions;
    }

    public async Task Run()
    {
        var basePath = await _appRegistry.GetBasePath() ?? ThrowHelper.ThrowInvalidOperationException<AbsolutePath>("Base path is not set");

        var app = _targetAppInfoOptions.Value.RcsBinName;
        var appPath = basePath / app;

        var processInfo = new ProcessStartInfo(appPath.Value)
        {
            WorkingDirectory = basePath.Value
        };

        Process.Start(processInfo);

        App.Exit();
    }
}
