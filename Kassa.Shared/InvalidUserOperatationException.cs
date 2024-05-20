using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public sealed class InvalidUserOperatationException(string message) : Exception(message)
{
    public string Icon
    {
        get; set;
    } = string.Empty;

    public string Description
    {
        get; set;
    } = string.Empty;
}
