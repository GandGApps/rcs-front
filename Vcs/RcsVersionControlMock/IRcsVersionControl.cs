using RcsVersionControlMock.DataAccess;

namespace RcsVersionControlMock;

public interface IRcsVersionControl
{

    public IRcsVersionChangesStore ChangesStore
    {
        get; set;
    }

    public IRcsVersionStore VersionStore
    {
        get; set;
    }

    public Task<Version?> GetCurrentVersion();
    public Task<IEnumerable<VersionChangeNode>> GetChangesFromCurrent(Version version);
    public Task<IEnumerable<VersionChangeNode>> GetAllValidChanges();
    public Task Update();
}
