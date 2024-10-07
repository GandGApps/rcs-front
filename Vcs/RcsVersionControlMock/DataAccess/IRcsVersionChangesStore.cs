namespace RcsVersionControlMock.DataAccess;

public interface IRcsVersionChangesStore
{
    public Task<IEnumerable<VersionChanges>> GetAllChanges();

    public Task<IEnumerable<VersionChanges>> GetChangesForVersion(Version version);

    public Task<IEnumerable<VersionChanges>> GetChangesBetweenVersions(Version versionA, Version versionB);

    public Task AddChanges(VersionChanges versionChanges);
    public Task SaveChanges(IEnumerable<VersionChanges> versionChanges);
    public Task<IEnumerable<VersionChanges>> GetChangesFromFirst();
}
