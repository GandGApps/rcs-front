using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RcsVersionControlMock.DataAccess;
using TruePath;

namespace RcsVersionControlMock.Json;
public sealed class JsonVersionStore : IRcsVersionStore
{
    private readonly AbsolutePath _rcsDirectory;
    private readonly AbsolutePath _filePath;
    private readonly ILogger _logger;

    public JsonVersionStore(AbsolutePath rcsDirectory, ILogger<JsonVersionStore> logger)
    {
        _rcsDirectory = rcsDirectory;
        _filePath = _rcsDirectory / "versions.json";
        _logger = logger;
    }

    public async Task<Version?> GetCurrentVersion()
    {
        var versions = await GetVersionsAsync();

        var currentVersion = versions.OrderByDescending(v => v).FirstOrDefault();

        _logger.LogInformation("Current version is {CurrentVersion}", currentVersion);

        return currentVersion;
    }

    public async Task<IEnumerable<Version>> GetVersionsAsync()
    {
        if (!File.Exists(_filePath.Value))
        {
            _logger.LogWarning("Versions file not found at {FilePath}", _filePath);
            return [];
        }

        _logger.LogInformation("Reading versions from {FilePath}", _filePath);

        using var file = File.Open(_filePath.Value, FileMode.Open);
        return await JsonSerializer.DeserializeAsync<IEnumerable<Version>>(file, RcsVCConstants.JsonSerializerOptions) ?? [];
    }

    public async Task<IEnumerable<Version>> GetVersionsBetweenAsync(Version a, Version b)
    {
        var versions = await GetVersionsAsync();

        _logger.LogInformation("Getting versions between {VersionA} and {VersionB}", a, b);

        return versions.Where(v => v >= a && v <= b).OrderBy(v => v);
    }

    public async Task SaveVersionsAsync(IEnumerable<Version> versions)
    {
        using var file = File.Open(_filePath.Value, FileMode.Create);
        await JsonSerializer.SerializeAsync(file, versions, RcsVCConstants.JsonSerializerOptions);

        _logger.LogInformation("Versions saved to {FilePath}", _filePath);
    }

    public async Task AddVersionAsync(Version version)
    {
        var versions = await GetVersionsAsync();

        _logger.LogInformation("Adding version {Version}", version);

        var list = versions.ToList();
        list.Add(version);

        await SaveVersionsAsync(list);
    }

    public async Task<Version?> GetFirstVersionsAsync()
    {
        var versions = await GetVersionsAsync();

        _logger.LogInformation("Getting first version");

        return versions.OrderBy(v => v).FirstOrDefault();
    }
}