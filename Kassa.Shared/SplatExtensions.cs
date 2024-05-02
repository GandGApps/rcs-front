using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.Shared;
public static class SplatExtensions
{
    /// <summary>
    /// Retrieves a required service of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <param name="services">The read-only dependency resolver from which to retrieve the service.</param>
    /// <returns>The requested service of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver services)
    {
        return services.GetService<T>() ?? throw new InvalidOperationException($"The service of type {typeof(T)} is not registered.");
    }
}
