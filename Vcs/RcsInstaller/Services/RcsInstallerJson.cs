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
using RcsInstaller.Progress;

namespace RcsInstaller.Services;

/// <summary>
/// The RcsInstallerJson class provides functionality for installing, updating,
/// and repairing the RCS application. It interacts with an API, manages shortcuts,
/// and registers app information.
/// </summary>
public sealed class RcsInstallerJson : IInstaller, IUpdater, IRepair
{
    private readonly IRcsApi _api;
    private readonly IShortcutCreator _shortcutCreator;
    private readonly IAppRegistry _appRegistry;
    private readonly ILogger<RcsInstallerJson> _logger;

    /// <summary>
    /// Initializes a new instance of RcsInstallerJson with required dependencies.
    /// </summary>
    /// <param name="api">The API interface for managing installation processes.</param>
    /// <param name="shortcutCreator">The shortcut creator instance.</param>
    /// <param name="appRegistry">The app registry for managing app properties and registration.</param>
    /// <param name="logger">The logger for recording installation logs.</param>
    public RcsInstallerJson(IRcsApi api, IShortcutCreator shortcutCreator, IAppRegistry appRegistry, ILogger<RcsInstallerJson> logger)
    {
        _api = api;
        _shortcutCreator = shortcutCreator;
        _appRegistry = appRegistry;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task InstallAsync(AbsolutePath path, Version currentVersion, bool createShortcut, Action<ProgressState> progress)
    {
        var loadedVersion = await LoadAndParseZip(path, currentVersion, progress);

        if (createShortcut)
        {
            await _shortcutCreator.CreateSrotcut(path, "RCS");
        }

        await _appRegistry.Register(loadedVersion, path);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(AbsolutePath path, Version currentVersion, Action<ProgressState> callback)
    {
        await LoadAndParseZip(path, currentVersion, callback);
    }

    /// <summary>
    /// Repairs the application by reinstalling based on registry properties.
    /// </summary>
    /// <param name="callback">The action to report repair progress.</param>
    public async Task RepairAsync(Action<ProgressState> callback)
    {
        var appRegistryProperties = await _appRegistry.GetProperties();

        if (appRegistryProperties is null)
        {
            ThrowHelper.ThrowInvalidOperationException("Failed to retrieve app registry properties. The app may not be installed.");
        }

        await InstallAsync(appRegistryProperties.Path, HelperExtensions.EmptyVersion, false, callback);
    }

    /// <summary>
    /// Loads and parses a ZIP archive from the specified path, extracting and processing contents.
    /// </summary>
    /// <param name="path">The destination path for the extracted files.</param>
    /// <param name="currentVersion">The current application version. If the application is not installed, the current version should be set to <see cref="HelperExtensions.EmptyVersion"/>.</param>
    /// <param name="progress">The action to report download and installation progress.</param>
    /// <returns>Returns the loaded version after parsing the archive.</returns>
    private async Task<Version> LoadAndParseZip(AbsolutePath path, Version currentVersion, Action<ProgressState> progress)
    {
        var stringVersion = HelperExtensions.EmptyVersion == currentVersion ? null : currentVersion.ToString();

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

    /// <summary>
    /// Parses a ZIP archive and extracts its contents to the specified path, updating progress.
    /// </summary>
    /// <param name="zipArchive">The ZIP archive to parse.</param>
    /// <param name="destinationPath">The path to extract the contents to.</param>
    /// <param name="progress">The action to report extraction progress.</param>
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
            if (path ==  App.CurrentPathAbsolute)
            {
                continue;
            }

            var directory = System.IO.Path.GetDirectoryName(path.Value);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                using var stream = entry.Open();

                using var fileStream = File.Create(path.Value);

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

    /// <summary>
    /// A helper structure for managing ZIP archive operations, particularly for
    /// retrieving and applying version changes.
    /// </summary>
    internal readonly ref struct ZipArchiveHelper(ZipArchive zipArchive)
    {
        private readonly ZipArchive _zipArchive = zipArchive;

        /// <summary>
        /// Retrieves the list of changes from the "changes.json" file within the archive.
        /// </summary>
        /// <returns>A collection of version changes to be applied.</returns>
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

        /// <summary>
        /// Retrieves a specific ZIP archive entry based on a change node, if the change action is AddOrModify.
        /// </summary>
        /// <param name="changeNode">The version change node indicating the file to retrieve.</param>
        /// <returns>The corresponding ZIP archive entry, or null if not applicable.</returns>
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