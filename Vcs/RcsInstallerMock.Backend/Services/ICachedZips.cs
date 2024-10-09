namespace RcsInstallerMock.Backend.Services;

public interface ICachedZips
{
    public Task<FileStream?> GetCachedZipAsync(Version a, Version b);
    public Task SaveZip(Version a, Version b, string path, TimeSpan removeAt);
}
