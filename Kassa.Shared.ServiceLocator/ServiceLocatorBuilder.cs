using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.ServiceLocator;
public static class ServiceLocatorBuilder
{
    internal static List<(Type, Func<object>)> _services = [];
    internal static List<(Type, Func<IServiceProvider, object>)> _scopedServices = [];

    public static void AddService<TService>(Func<object> func) where TService : class
    {
        _services.Add((typeof(TService), func));
    }

    public static void AddScopedService<TService>(Func<IServiceProvider, object> func) where TService : class
    {
        _scopedServices.Add((typeof(TService), func));
    }

    public static void Clear()
    {
        _services.Clear();
        _scopedServices.Clear();
    }

    /// <summary>
    /// Call this method after all services are added
    /// </summary>
    public static void SetLocator()
    {
        var services = new Dictionary<Type, Func<object>>();

        foreach (var (type, factory) in _services)
        {
            services[type] = factory;
        }

        // Find all duplicate types 
        // and register them as IEnumerable<T>

        var duplicateTypes = _services
            .GroupBy(x => x.Item1)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key);

        foreach (var type in duplicateTypes)
        {
            services[type] = () => _services
                .Where(x => x.Item1 == type)
                .Select(x => x.Item2())
                .ToArray();
        }


        RcsLocator.SetLocator(services.ToFrozenDictionary());
        RcsLocator.SetScopeFactory(() => _scopedServices);
    }
}
