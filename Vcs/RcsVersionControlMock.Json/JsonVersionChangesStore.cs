using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RcsVersionControlMock.DataAccess;
using TruePath;

namespace RcsVersionControlMock.Json;

[SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
[SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
public sealed class JsonVersionChangesStore : IRcsVersionChangesStore
{
    private readonly AbsolutePath _rcsPath;
    private readonly AbsolutePath _filePath;
    private readonly IRcsVersionStore _rcsVersionStore;
    private readonly ILogger _logger;

    public JsonVersionChangesStore(AbsolutePath rcsPath, IRcsVersionStore rcsVersionStore, ILogger<JsonVersionChangesStore> logger)
    {
        _rcsPath = rcsPath;
        _filePath = rcsPath / "changes.json";
        _rcsVersionStore = rcsVersionStore;
        _logger = logger;
    }

    public async Task<IEnumerable<VersionChanges>> GetAllChanges()
    {
        if (!File.Exists(_filePath.Value))
        {
            _logger.LogWarning("Changes file not found at {FilePath}", _filePath);
            return [];
        }

        _logger.LogInformation("Reading changes from {FilePath}", _filePath);
        using var jsonFile = File.OpenRead(_filePath.Value);
        return (await JsonSerializer.DeserializeAsync<IEnumerable<VersionChanges>>(jsonFile, RcsVCConstants.JsonSerializerOptions))!;
    }

    public async Task<IEnumerable<VersionChanges>> GetChangesForVersion(Version version)
    {
        var currentVersion = await _rcsVersionStore.GetCurrentVersion();

        if (currentVersion is null)
        {
            _logger.LogWarning("Current version not found");
            return [];
        }

        return await GetChangesBetweenVersions(currentVersion, version);
    }

    public async Task<IEnumerable<VersionChanges>> GetChangesBetweenVersions(Version versionA, Version versionB)
    {
        // swap versions if versionA is greater than versionB
        if (versionA > versionB)
        {
            (versionB, versionA) = (versionA, versionB);
        }

        _logger.LogInformation("Getting changes between versions {VersionA} and {VersionB}", versionA, versionB);

        var allChanges = await GetAllChanges();

        var filteredChanges = allChanges
            .Where(vc => vc.Version > versionA && vc.Version <= versionB)
            .OrderByDescending(vc => vc.Version)
            .ToList();

        _logger.LogInformation("Found {Count} changes between versions {VersionA} and {VersionB}", filteredChanges.Count, versionA, versionB);

        var latestChangesDict = new Dictionary<string, VersionChangeNode>();

        foreach (var versionChange in filteredChanges)
        {
            var updatedChanges = new List<VersionChangeNode>();

            foreach (var changeNode in versionChange.Changes)
            {
                if (!latestChangesDict.ContainsKey(changeNode.Path))
                {
                    latestChangesDict[changeNode.Path] = changeNode;
                    updatedChanges.Add(changeNode);
                }
            }

            versionChange.Changes = updatedChanges;
        }

        _logger.LogInformation("Filtered changes to {Count} changes between versions {VersionA} and {VersionB}", filteredChanges.Count, versionA, versionB);

        return filteredChanges;
    }

    public async Task SaveChanges(IEnumerable<VersionChanges> versionChanges)
    {
        using var file = File.Open(_filePath.Value, FileMode.Create);
        await JsonSerializer.SerializeAsync(file, versionChanges, RcsVCConstants.JsonSerializerOptions);
    }

    public async Task AddChanges(VersionChanges versionChanges)
    {
        var allChanges = await GetAllChanges();
        var list = allChanges.ToList();

        list.Add(versionChanges);

        await SaveChanges(list);
    }

    public async Task<IEnumerable<VersionChanges>> GetChangesFromFirst()
    {
        var currentVersion = await _rcsVersionStore.GetCurrentVersion();
        var firstVersion = RcsVCConstants.EmptyVersion;

        if (firstVersion == null || currentVersion == null)
        {
            return [];
        }

        return await GetChangesBetweenVersions(firstVersion, currentVersion);
    }
}