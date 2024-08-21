using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.ServiceLocator;
public readonly struct FrozenServiceLocator(FrozenDictionary<Type, Func<object>> servicesFactory) : IServiceProvider, IEquatable<FrozenServiceLocator>
{

    public static FrozenServiceLocator Empty
    {
        get;
    } = new FrozenServiceLocator(FrozenDictionary<Type, Func<object>>.Empty);

    private readonly FrozenDictionary<Type, Func<object>> _servicesFactory = servicesFactory;

    public object? GetService(Type serviceType)
    {
        if (_servicesFactory.TryGetValue(serviceType, out var factory))
        {
            return factory();
        }

        if (serviceType.IsGenericType)
        {
            var genericServiceType = serviceType.GetGenericTypeDefinition();

            if (_servicesFactory.TryGetValue(genericServiceType, out factory))
            {
                var genericFactory = (Func<Type[],object>)factory();

                return genericFactory(serviceType.GenericTypeArguments);
            }
        }

        return null;
    }

    public T? GetService<T>() where T : class
    {
        var service = GetService(typeof(T));

        if (service == null)
        {
            return null;
        }

        // This is safe because we know that the service is of type T
        return Unsafe.As<T>(service);
    }

    public T GetRequiredService<T>() where T : class
    {
        var service = GetService<T>();

        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        }

        return service;
    }

    public override bool Equals(object? obj) => obj is FrozenServiceLocator locator && Equals(locator);
    public bool Equals(FrozenServiceLocator other) => _servicesFactory.Equals(other._servicesFactory);

    public static bool operator ==(FrozenServiceLocator left, FrozenServiceLocator right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FrozenServiceLocator left, FrozenServiceLocator right)
    {
        return !(left == right);
    }

    public override int GetHashCode() => _servicesFactory.GetHashCode();
}
