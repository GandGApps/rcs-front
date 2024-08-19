using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.ServiceLocator;
public static class ServiceLocatorBuilder
{
    internal static Dictionary<Type, Func<object>> _services = [];
    public static void AddService<TService>(Func<object> func) where TService : class
    {
        _services[typeof(TService)] = func;
    }

    public static void SetLocator()
    {
        RcsLocator.SetLocator(_services.ToFrozenDictionary());
    }
}
