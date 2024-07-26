using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public interface IInitializable
{
    public bool IsInitialized
    {
        get;
    }

    public ValueTask Initialize();
}
