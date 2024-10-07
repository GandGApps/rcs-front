using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsVersionControlMock.DataAccess;
public class VersionChangeNode
{
    public required string Sha
    {
        get; set;
    }

    public required string Path
    {
        get; set;
    }

    public long LengthOfFile
    {
        get; set;
    }

    public VersionChangeNodeAction ChangeType
    {
        get; set;
    }

}
