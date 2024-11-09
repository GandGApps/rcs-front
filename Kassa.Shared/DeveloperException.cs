using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
/// <summary>
/// Эти ошибки точно будут(должны быть) перехвачены и обработаны. 
/// </summary>
public sealed class DeveloperException(string message): Exception(message)
{

    [DoesNotReturn]
    public static void Throw(string message)
    {
        throw new DeveloperException(message);
    }

    [DoesNotReturn]
    public static T ThrowDeveloperException<T>(string message)
    {
        throw new DeveloperException(message);
    }

}
