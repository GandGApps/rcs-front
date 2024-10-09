namespace RcsInstallerMock.Backend.Models;

public class ZipCacheEntry
{
    public required Version VersionA
    {
        get; set;
    }
    public required Version VersionB
    {
        get; set;
    }
    public required string ZipFilePath
    {
        get; set;
    }
    public required DateTime ExpirationTime
    {
        get; set;
    }
}