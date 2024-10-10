using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public sealed class FatalException(Exception exception) : Exception("An unhandled exception occurred", exception)
{
}
