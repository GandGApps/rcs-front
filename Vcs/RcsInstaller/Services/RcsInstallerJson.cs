using RcsVersionControlMock.DataAccess;
using RcsVersionControlMock.Json;
using Splat;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RcsInstaller;
using System.IO;
using Microsoft.Win32;
using Avalonia.Input;
using TruePath;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Avalonia.Controls.Shapes;
using Avalonia.OpenGL;
using System.Diagnostics;
using RcsInstaller.Vms;

namespace RcsInstaller.Services;
public sealed class RcsInstallerJson : IInstaller, IUpdater, IRepair
{
    private readonly IRcsApi _api;
    private readonly IShortcutCreator _shortcutCreator;
    private readonly ILogger<RcsInstallerJson> _logger;

    public RcsInstallerJson(IRcsApi api, IShortcutCreator shortcutCreator, ILogger<RcsInstallerJson> logger)
    {
        _api = api;
        _shortcutCreator = shortcutCreator;
        _logger = logger;
    }

    public async Task InstallAsync(AbsolutePath path, Version version, bool createShortcut, Action<ProgressState> progress)
    {
        var loadedVersion = await LoadAndParseZip(path, version, progress);

        if (createShortcut)
        {
            await _shortcutCreator.CreateSrotcut(path, "RCS");
        }

        // Create app info in registry
#pragma warning disable CA1416 // Validate platform compatibility
        var key = Registry.LocalMachine.OpenSubKey(LauncherConstants.RegistryKeyPath);

        // Check is key exists
        if (key != null)
        {
            try
            {
                var keyVersion = key.GetValue("DisplayVersion");
                if (keyVersion is string versionString)
                {
                    var currentVersion = Version.Parse(versionString);

                    if (loadedVersion != currentVersion)
                    {
                        _logger.LogInformation("Version changed from {CurrentVersion} to {NewVersion}, in registry", currentVersion, version);
                        key.SetValue("DisplayVersion", loadedVersion.ToString());
                        return;
                    }
                }
            }
            catch
            {
                // if something goes wrong, need to dispose
                // but don't need to dispose if all is ok, not here
                key?.Dispose();
                throw;
            }
        }

        using (key ??= Registry.LocalMachine.CreateSubKey(LauncherConstants.RegistryKeyPath))
        { 
            if (key != null)
            {
                _logger.LogInformation("Creating registry key for RCS");
                key.SetValue("DisplayName", "Супер мега касса с кодовым именем RCS");
                key.SetValue("DisplayVersion", loadedVersion.ToString());
                key.SetValue("Publisher", "Gang bang");
                key.SetValue("InstallLocation", path / "Kassa.Wpf.exe");
                key.SetValue("UninstallString", path / "Kassa.Launcher.exe --remove");
                key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
            }
        }

#pragma warning restore CA1416 // Validate platform compatibility
    }

    public async Task UpdateAsync(AbsolutePath path, Version version, Action<ProgressState> value)
    {
        await LoadAndParseZip(path, version, value);
    }

    public Task RepairAsync(Action<ProgressState> value)
    {
        // Get current app

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="version"></param>
    /// <param name="progress"></param>
    /// <returns>return loaded version</returns>
    private async Task<Version> LoadAndParseZip(AbsolutePath path, Version version, Action<ProgressState> progress)
    {
        var stringVersion = HelperExtensions.EmptyVersion == version ? null : version.ToString();

        var httpContent = await _api.InstallLatest(stringVersion);

        var zipStream = await httpContent.ReadAsStreamAsync();

        var tempZipFilePath = System.IO.Path.GetTempFileName();

        await using (var progressStream = new ProgressStream(zipStream, httpContent.Headers.ContentLength))
        {
            progressStream.UpdateProgress += (s, e) => progress(new("Скачивание...", e.Progress / 2));

            await using var tempZipStream = File.OpenWrite(tempZipFilePath);
            await progressStream.CopyToAsync(tempZipStream);
        }

        using (var zipArchive = ZipFile.OpenRead(tempZipFilePath))
        {
            ParseZip(zipArchive, path, progress);
        }

        var loadedVersion = await _api.GetVersion();

        // Remove temp file
        try
        {
            File.Delete(tempZipFilePath);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete temp file");
        }

        return loadedVersion;
    }

    private static void ParseZip(ZipArchive zipArchive, AbsolutePath destinationPath, Action<ProgressState> progress)
    {
        var helper = new ZipArchiveHelper(zipArchive);

        var changes = helper.GetChanges();

        var totalProgress = changes.Count();
        var currentProgress = 0;

        foreach (var change in changes)
        {
            var entry = helper.GetChange(change);

            if (entry == null)
            {
                continue;
            }

            var path = destinationPath / change.Path;

            // Sometimes, 'change' is a directory rather than a file.
            // We need to skip it because this is a bug in changes.json.
            // change.Path should always represent a file, not a directory.
            if (Directory.Exists(path.Value))
            {
                continue;
            }

            // Skip local files
            if (path.Value.Contains(".local."))
            {
                continue;
            }

            // Skip the installer itself
            if (path.Value.EndsWith("RcsInstaller.exe"))
            {
                continue;
            }

            var directory = System.IO.Path.GetDirectoryName(path.Value);

            if (!string.IsNullOrEmpty(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            try
            {
                using var stream = entry.Open();

                using var fileStream = System.IO.File.Create(path.Value);

                stream.CopyTo(fileStream);

                currentProgress++;
                var progressValue = (double)currentProgress / totalProgress;
                var progressState = new ProgressState("Установка...", 0.5 + (progressValue / 2));

                progress(progressState);
            }
            catch(Exception e)
            {
                ThrowHelper.ThrowInvalidOperationException($"Failed to extract {change.Path}", e);
            }
        }

    }

    

    internal readonly ref struct ZipArchiveHelper(ZipArchive zipArchive)
    {
        private readonly ZipArchive _zipArchive = zipArchive;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
        public readonly IEnumerable<VersionChangeNode> GetChanges()
        {
            var changes = _zipArchive.GetEntry("changes.json");

            if (changes == null)
            {
                ThrowHelper.ThrowInvalidOperationException("Required file 'changes.json' not found in the archive. This file is necessary for processing changes.");
            }

            using var stream = changes.Open();
            var jsonParsedChanges = JsonSerializer.Deserialize<IEnumerable<VersionChangeNode>>(stream, RcsVCConstants.JsonSerializerOptions);

            return jsonParsedChanges ?? [];
        }

        public readonly ZipArchiveEntry? GetChange(VersionChangeNode changeNode)
        {
            if (changeNode.ChangeType == VersionChangeNodeAction.AddOrModigy)
            {
                return _zipArchive.GetEntry(changeNode.Path);
            }

            return null;

        }
    }

}