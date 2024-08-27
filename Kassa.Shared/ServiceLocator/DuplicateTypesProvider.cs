using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.ServiceLocator;
public sealed class DuplicateTypesProvider(FrozenDictionary<Type, Func<object>> enumerableFactory) : IServiceProvider
{
    private readonly FrozenDictionary<Type, Func<object>> _enumerableFactory = enumerableFactory;

    public object? GetService(Type serviceType)
    {
        return _enumerableFactory.TryGetValue(serviceType, out var factory) ? factory() : null;
    }

    public IEnumerable<T>? GetService<T>()
    {
        var service = GetService(typeof(IEnumerable<T>));

        if (service == null)
        {
            return null;
        }

        return (IEnumerable<T>)service;
    }
}
