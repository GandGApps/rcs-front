using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RcsVersionControlMock.DataAccess;
using TruePath;

namespace RcsVersionControlMock.Json;
public sealed class RcsVersionControl : IRcsVersionControl
{
    private readonly AbsolutePath _appPath;
    private readonly string _appName;
    private readonly IRcsVersionChangesStore _changesStore;
    private readonly IRcsVersionStore _versionStore;
    private readonly ILogger _logger;
    private readonly AbsolutePath _appDirectory;
    private readonly AbsolutePath _rcsDirectory;

    public IRcsVersionChangesStore ChangesStore
    {
        get => _changesStore;
        set => throw new NotSupportedException("This property only for read only!");
    }

    public IRcsVersionStore VersionStore
    {
        get => _versionStore;
        set => throw new NotSupportedException("This property only for read only!");
    }

    public RcsVersionControl(IRcsVersionChangesStore changesStore, IRcsVersionStore versionStore, ILogger<RcsVersionControl> logger, AbsolutePath appPath)
    {
        _changesStore = changesStore;
        _versionStore = versionStore;

        _appPath = appPath;
        _appName = appPath.FileName;
        _appDirectory = appPath.Parent ?? throw new ArgumentException($"{nameof(appPath)} must have parent directory", nameof(appPath));
        _rcsDirectory = _appDirectory / ".rcs";

        if (!Directory.Exists(_rcsDirectory.Value))
        {
            Directory.CreateDirectory(_rcsDirectory.Value);
        }

        _logger = logger;
    }

    public async Task<Version?> GetCurrentVersion()
    {
        return await _versionStore.GetCurrentVersion();
    }

    public async Task<IEnumerable<VersionChangeNode>> GetChangesFromCurrent(Version version)
    {
        var currentVersion = await GetCurrentVersion();

        if (currentVersion == null)
        {
            _logger.LogWarning("Current version is null");
            return [];
        }

        _logger.LogInformation("Current version is {Version}", currentVersion);
        var versionChanges = await _changesStore.GetChangesBetweenVersions(version, currentVersion);

        return versionChanges.SelectMany(vc => vc.Changes);
    }

    public async Task Update()
    {
        _logger.LogInformation("Starting update process.");

        var assembly = FileVersionInfo.GetVersionInfo(_appPath.Value);
        _logger.LogInformation("Loaded assembly version info for application.");

        var assemblyVersion = Version.TryParse(assembly.ProductVersion, out var vers)
            ? vers
            : Version.TryParse(assembly.FileVersion, out vers)
                ? vers
                : throw new NotSupportedException("Could not parse version from assembly.");

        _logger.LogInformation("Parsed assembly version: {AssemblyVersion}", assemblyVersion);

        var currentVersion = await GetCurrentVersion();

        if (currentVersion == null)
        {
            _logger.LogInformation("No current version found. Initializing changes collection.");

            var changes = new List<VersionChangeNode>();
            var files = Directory.GetFiles(_appDirectory.Value, "*.*", SearchOption.AllDirectories)
                                 .Where(file => !file.StartsWith(_rcsDirectory.Value));

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                //var path =  fileInfo.FullName.Replace(_appDirectory.Value, string.Empty).Trim('\\');
                var sha = CalculateFileSha(fileInfo.FullName);

                var change = new VersionChangeNode
                {
                    Path = GetLocalPath(fileInfo.FullName),
                    LengthOfFile = fileInfo.Length,
                    Sha = sha,
                    ChangeType = VersionChangeNodeAction.AddOrModigy
                };

                _logger.LogInformation("Detected new or modified file: {FilePath}, SHA: {Sha}", change.Path, sha);
                changes.Add(change);
            }

            await SaveChanges(assemblyVersion, changes);
            _logger.LogInformation("Changes saved for version {Version}.", assemblyVersion);
            return;
        }

        if (assemblyVersion > currentVersion)
        {
            _logger.LogInformation("New version detected. Current version: {CurrentVersion}, New version: {NewVersion}", currentVersion, assemblyVersion);

            var nextVersion = assemblyVersion;

            var allChanges = await _changesStore.GetChangesFromFirst();
            _logger.LogInformation("Retrieved all changes from first version.");

            var changes = new List<VersionChangeNode>();

            var files = Directory.GetFiles(_appDirectory.Value, "*.*", SearchOption.AllDirectories)
                                 .Where(file => !file.StartsWith(_rcsDirectory.Value));

            var checkedChanged = allChanges.SelectMany(vc => vc.Changes).ToDictionary(vc => vc.Path, vc => false);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var path = GetLocalPath(fileInfo.FullName);
                var change = allChanges.SelectMany(vc => vc.Changes).FirstOrDefault(c => c.Path == path);
                VersionChangeNode? changeNode = null;

                if (change != null)
                {
                    if (fileInfo.Length != change.LengthOfFile)
                    {
                        changeNode = AddChange(fileInfo, change);
                        _logger.LogInformation("File size changed for {FilePath}.", changeNode.Path);
                    }

                    var sha = CalculateFileSha(fileInfo.FullName);
                    if (change.Sha != sha)
                    {
                        changeNode = AddChange(fileInfo, change, sha);
                        _logger.LogInformation("SHA changed for {FilePath}. New SHA: {Sha}", path, sha);
                    }

                    if (changeNode != null)
                    {
                        changes.Add(changeNode);
                        _logger.LogInformation("File modified: {FilePath}.", path);
                    }

                    checkedChanged[change.Path] = true;
                }

                if (change == null)
                {
                    changeNode = AddChange(fileInfo, null);
                    changes.Add(changeNode);
                    _logger.LogInformation("New file detected: {FilePath}.", path);
                }
            }

            var uncheckedChanges = checkedChanged.Where(kvp => !kvp.Value).Select(kvp => kvp.Key);

            foreach (var change in uncheckedChanges)
            {
                var changeNode = allChanges.SelectMany(vc => vc.Changes).FirstOrDefault(c => c.Path == change);
                if (changeNode != null)
                {
                    changeNode.ChangeType = VersionChangeNodeAction.Delete;
                    changes.Add(changeNode);
                    _logger.LogInformation("File deleted: {FilePath}.", change);
                }
            }

            await SaveChanges(nextVersion, changes);
            _logger.LogInformation("Changes saved for version {Version}.", nextVersion);
        }
        else
        {
            _logger.LogInformation("No new version detected. Current version: {CurrentVersion}.", currentVersion);
        }

        _logger.LogInformation("Update process completed.");
    }

    private async Task SaveChanges(Version version, IEnumerable<VersionChangeNode> versionChangeNodes)
    {
        var versionChanges = new VersionChanges
        {
            Version = version,
            Changes = versionChangeNodes
        };

        await _changesStore.AddChanges(versionChanges);
        await _versionStore.AddVersionAsync(version);
    }

    private VersionChangeNode AddChange(FileInfo fileInfo, VersionChangeNode? currentVersionChangeNode, string? sha = null)
    {
        sha ??= CalculateFileSha(fileInfo.FullName);
        return new VersionChangeNode
        {
            Path = GetLocalPath(fileInfo.FullName),
            LengthOfFile = fileInfo.Length,
            Sha = sha,
            ChangeType = VersionChangeNodeAction.AddOrModigy
        };
    }

    private static string CalculateFileSha(string fullPath)
    {
        var fileContent = File.ReadAllBytes(fullPath);
        var gitObjectHeader = $"blob {fileContent.Length}\0";
        var headerBytes = Encoding.UTF8.GetBytes(gitObjectHeader);

        using var sha1 = SHA1.Create();
        sha1.TransformBlock(headerBytes, 0, headerBytes.Length, headerBytes, 0);
        sha1.TransformFinalBlock(fileContent, 0, fileContent.Length);
        return BitConverter.ToString(sha1.Hash!).Replace("-", "").ToLower();
    }

    public async Task<IEnumerable<VersionChangeNode>> GetAllValidChanges()
    {
        var currentVersion = await GetCurrentVersion();
        var firstVersion = new Version(0, 0);

        if (firstVersion == null || currentVersion == null)
        {
            _logger.LogWarning("First or current version is null. First version :{FirstVersion}; Current is {CurrentVersion}", firstVersion, currentVersion);
            return [];
        }

        var changes = await _changesStore.GetChangesBetweenVersions(firstVersion, currentVersion);

        _logger.LogInformation("Retrieved changes between first and current version.");

        return changes.SelectMany(vc => vc.Changes);
    }

    private string GetLocalPath(string fullPath)
    {
        var fileInfoFullNameAbsolutePath = new AbsolutePath(fullPath);
        var localPath = fileInfoFullNameAbsolutePath.RelativeTo(_appDirectory).Value;

        _logger.LogDebug("Local path for {FullPath} is {LocalPath}", fullPath, localPath);

        return localPath;

    }
}