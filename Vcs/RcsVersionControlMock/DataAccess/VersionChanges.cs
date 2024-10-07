using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsVersionControlMock.DataAccess;

public class VersionChanges
{
    public required Version Version
    {
        get; set;
    }

    public virtual required IEnumerable<VersionChangeNode> Changes
    {
        get; set;
    }
}
