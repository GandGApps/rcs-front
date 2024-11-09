using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    [DoesNotReturn]
    public static void Throw(string message, string icon, string description)
    {
        throw new InvalidUserOperatationException(message)
        {
            Icon = icon,
            Description = description
        };
    }

    [DoesNotReturn]
    public static void Throw(string message)
    {
        throw new InvalidUserOperatationException(message);
    }

    [DoesNotReturn]
    public static void Throw(string message, string description)
    {
        throw new InvalidUserOperatationException(message)
        {
            Description = description
        };
    }
}
