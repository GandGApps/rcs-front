using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;

/// <summary>
/// Used for services that do not require initialization
/// </summary>
public class EmptyInitializable : IInitializable
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static readonly EmptyInitializable Instance = new();

    public bool IsInitialized => true;

    public ValueTask Initialize() => ValueTask.CompletedTask;
}
