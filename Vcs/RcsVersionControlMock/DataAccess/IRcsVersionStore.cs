using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsVersionControlMock.DataAccess;
public interface IRcsVersionStore
{
    public Task<Version?> GetCurrentVersion();

    public Task<IEnumerable<Version>> GetVersionsAsync();

    public Task<IEnumerable<Version>> GetVersionsBetweenAsync(Version a, Version b);
    public Task SaveVersionsAsync(IEnumerable<Version> versions);

    public Task AddVersionAsync(Version version);
    public Task<Version?> GetFirstVersionsAsync();
}
