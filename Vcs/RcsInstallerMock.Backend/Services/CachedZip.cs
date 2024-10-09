using System.Text.Json.Serialization;
using System.Text.Json;
using RcsInstallerMock.Backend.Models;

namespace RcsInstallerMock.Backend.Services;

public sealed class CachedZips : ICachedZips
{
    internal static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        TypeInfoResolver = CachedZipContext.Default,
    };

    private readonly string _cacheFilePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public CachedZips(string cacheFilePath)
    {
        _cacheFilePath = cacheFilePath;
    }

    public async Task<FileStream?> GetCachedZipAsync(Version a, Version b)
    {
        if (a > b)
        {
            (a, b) = (b, a);
        }

        await _semaphore.WaitAsync();
        try
        {
            var entries = await LoadCacheAsync();

            var now = DateTime.UtcNow;

            var entry = entries.Find(e => e.VersionA.Equals(a) && e.VersionB.Equals(b));

            if (entry != null)
            {
                if (entry.ExpirationTime > now && File.Exists(entry.ZipFilePath))
                {
                    return File.OpenRead(entry.ZipFilePath);
                }
                else
                {
                    // Remove expired entry and corresponding file
                    entries.Remove(entry);
                    await SaveCacheAsync(entries);

                    if (File.Exists(entry.ZipFilePath))
                    {
                        File.Delete(entry.ZipFilePath);
                    }
                }
            }

            return null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveZip(Version a, Version b, string path, TimeSpan removeAt)
    {
        if (a > b)
        {
            (a, b) = (b, a);
        }

        await _semaphore.WaitAsync();
        try
        {
            var entries = await LoadCacheAsync();

            // Remove existing entry for these versions, if any
            entries.RemoveAll(e => e.VersionA.Equals(a) && e.VersionB.Equals(b));

            var cacheEntry = new ZipCacheEntry
            {
                VersionA = a,
                VersionB = b,
                ZipFilePath = path,
                ExpirationTime = DateTime.UtcNow.Add(removeAt)
            };

            entries.Add(cacheEntry);

            await SaveCacheAsync(entries);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<ZipCacheEntry>> LoadCacheAsync()
    {
        if (File.Exists(_cacheFilePath))
        {
            using var jsonStream = File.OpenRead(_cacheFilePath);
            var entries = await JsonSerializer.DeserializeAsync<List<ZipCacheEntry>>(jsonStream, _jsonSerializerOptions);
            if (entries != null)
            {
                // Remove expired entries and corresponding files
                var now = DateTime.UtcNow;
                entries.RemoveAll(entry =>
                {
                    if (entry.ExpirationTime <= now)
                    {
                        if (File.Exists(entry.ZipFilePath))
                        {
                            File.Delete(entry.ZipFilePath);
                        }
                        return true;
                    }
                    return false;
                });
                return entries;
            }
        }
        return new List<ZipCacheEntry>();
    }

    private async Task SaveCacheAsync(List<ZipCacheEntry> entries)
    {
        using var jsonStream = File.Create(_cacheFilePath);
        await JsonSerializer.SerializeAsync(jsonStream, entries, _jsonSerializerOptions);
    }
}


[JsonSerializable(typeof(List<ZipCacheEntry>))]
internal sealed partial class CachedZipContext : JsonSerializerContext;