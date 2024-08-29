using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public sealed class FatalException: Exception
{
    public FatalException(Exception exception): base("An unhandled exception occurred", exception)
    {

    }
}
