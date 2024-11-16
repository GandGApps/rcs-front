using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using RcsInstaller.Configurations;
using TruePath;

namespace RcsInstaller.Services;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
internal sealed class WndAppRegistry : IAppRegistry
{
    public const string UninstallRegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
    public const string DisplayNameKey = "DisplayName";
    public const string DisplayVersionKey = "DisplayVersion";
    public const string InstallLocationKey = "InstallLocation";
    public const string PublisherKey = "Publisher";
    public const string UninstallStringKey = "UninstallString";
    public const string NoModifyKey = "NoModify";
    public const string NoRepairKey = "NoRepair";

    private readonly ILogger<WndAppRegistry> _logger;
    private readonly IOptions<TargetAppInfo> _targetAppInfoOptions;

    public WndAppRegistry(ILogger<WndAppRegistry> logger, IOptions<TargetAppInfo> targetAppInfoOptions)
    {
        _logger = logger;
        _targetAppInfoOptions = targetAppInfoOptions;
    }

    public async ValueTask<AbsolutePath?> GetBasePath()
    {
        var registryKeyPath = UninstallRegistryKeyPath + _targetAppInfoOptions.Value.Name;

        if (!await IsRegistered())
        {
            return null;
        }

        using var key = Registry.LocalMachine.OpenSubKey(registryKeyPath)!;

        var path = (string?)key.GetValue(InstallLocationKey);

        if (path == null)
        {
            _logger.LogError("Install location not found in registry");
        }

        return path is not null ? new(path) : null;
    }

    public async ValueTask<AppRegistryProperties?> GetProperties()
    {
        var registryKeyPath = UninstallRegistryKeyPath + _targetAppInfoOptions.Value.Name;

        if (!await IsRegistered())
        {
            return null;
        }

        using var key = Registry.LocalMachine.OpenSubKey(registryKeyPath)!;

        var properties = new Dictionary<string, object>();

        foreach (var valueName in key.GetValueNames())
        {
            properties[valueName] = key.GetValue(valueName)!;
        }

        var versionString = (string)properties[DisplayVersionKey];
        var version = Version.Parse(versionString!);
        var path = (string)properties[InstallLocationKey];

        return new AppRegistryProperties(version, new(path), properties);
    }

    public async ValueTask<Version?> GetVersion()
    {
        var properties = await GetProperties();

        return properties?.Version ?? new Version();
    }

    public ValueTask<bool> IsRegistered()
    {
        var registryKeyPath = UninstallRegistryKeyPath + _targetAppInfoOptions.Value.Name;

        using var key = Registry.LocalMachine.OpenSubKey(registryKeyPath);
        return ValueTask.FromResult(key != null);

    }

    public ValueTask Register(Version loadedVersion, AbsolutePath path, IReadOnlyDictionary<string, object> properties)
    {
        var targetAppInfo = _targetAppInfoOptions.Value;
        var registryKeyPath = UninstallRegistryKeyPath + targetAppInfo.Name;

        using var regKey = Registry.LocalMachine.CreateSubKey(registryKeyPath);

        _logger.LogInformation("Creating registry key for RCS");

        regKey.SetValue(DisplayNameKey, targetAppInfo.DisplayName);
        regKey.SetValue(DisplayVersionKey, loadedVersion.ToString());
        regKey.SetValue(PublisherKey, targetAppInfo.Publisher);
        regKey.SetValue(InstallLocationKey, path / targetAppInfo.RcsBinName);
        regKey.SetValue(UninstallStringKey, path / targetAppInfo.RemovePath);
        regKey.SetValue(NoModifyKey, targetAppInfo.IsModifiable ? 0 : 1, RegistryValueKind.DWord);
        regKey.SetValue(NoRepairKey, targetAppInfo.IsRepairable ? 0 : 1, RegistryValueKind.DWord);

        foreach (var (key, value) in properties)
        {
            regKey.SetValue(key, value);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask UnRegister()
    {
        var registryKeyPath = UninstallRegistryKeyPath + _targetAppInfoOptions.Value.Name;

        Registry.LocalMachine.DeleteSubKeyTree(registryKeyPath, false);

        return ValueTask.CompletedTask;
    }
}
