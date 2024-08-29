using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public sealed class UnhandledErrorExceptionEventArgs(Exception exception)
{
    public Exception Exception => exception;

    public bool Handled
    {
        get; set;
    }
}
