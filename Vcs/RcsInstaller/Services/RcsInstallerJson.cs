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

namespace RcsInstaller.Services;
public sealed class RcsInstallerJson : IInstaller, IEnableLogger
{
    public async Task InstallAsync(string path, Version version, bool createShortcut, Action<ProgressState> progress)
    {
        var api = Locator.Current.GetRequiredService<IRcsApi>();


        var stringVersion = HelperExtensions.EmptyVersion == version ? null : version.ToString();

        var httpContent = await api.InstallLatest(stringVersion);

        var zipStream = await httpContent.ReadAsStreamAsync();

        var tempZipFilePath = Path.GetTempFileName();

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

        var loadedVersion = await api.GetVersion();

        // Remove temp file
        try
        {
            File.Delete(tempZipFilePath);
        }
        catch (Exception e)
        {
            this.Log().Error(e, "Failed to delete temp file");
        }

        if (createShortcut)
        {
            var shortcutCreator = Locator.Current.GetRequiredService<IShortcutCreator>();

            await shortcutCreator.CreateSrotcut(path, "RCS");
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
                        this.Log().Info("Version changed from {CurrentVersion} to {NewVersion}, in registry", currentVersion, version);
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
                this.Log().Info("Creating registry key for RCS");
                key.SetValue("DisplayName", "Супер мега касса с кодовым именем RCS");
                key.SetValue("DisplayVersion", loadedVersion.ToString());
                key.SetValue("Publisher", "Gang bang");
                key.SetValue("InstallLocation", Path.Combine(path, "Kassa.Wpf.exe"));
                key.SetValue("UninstallString", Path.Combine(path, "Kassa.Launcher.exe --remove"));
                key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
            }
        }

#pragma warning restore CA1416 // Validate platform compatibility
    }

    public static void ParseZip(ZipArchive zipArchive, string destinationPath, Action<ProgressState> progress)
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

            var path = Path.Combine(destinationPath, change.Path);

            var directory = System.IO.Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            using var stream = entry.Open();

            using var fileStream = System.IO.File.Create(path);

            stream.CopyTo(fileStream);

            currentProgress++;
            var progressValue = (double)currentProgress / totalProgress;
            var progressState = new ProgressState("Установка...", 0.5 + (progressValue / 2));

            progress(progressState);
        }

    }

    internal readonly struct ZipArchiveHelper(ZipArchive zipArchive)
    {
        private readonly ZipArchive _zipArchive = zipArchive;

        public readonly IEnumerable<VersionChangeNode> GetChanges()
        {
            var changes = _zipArchive.GetEntry("changes.json");

            if (changes == null)
            {
                throw new Exception("No changes.json found");
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