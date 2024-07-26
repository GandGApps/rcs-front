using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.RcsLocator;
public sealed class RcsLocatorBuilder
{
    internal FrozenDictionary<Type, ServiceDesciptor> FrozenServices;
    internal Dictionary<Type, ServiceDesciptor> Services = [];

    internal FrozenDictionary<Type, ServiceDesciptor> FrozenScopedServices;
    internal Dictionary<Type, ServiceDesciptor> ScopedServices = [];

    public void AddConstant<T>(T value)
    {
        Services[typeof(T)] = new(typeof(T), null, value);
    }

    /// <summary>
    /// If you have parameterless constructor, you can use this method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddScoped<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>() where T : class
    {
        ScopedServices[typeof(T)] = new(typeof(T), () => Activator.CreateInstance<T>()!, null);
    }

    public void AddTransient<T>(Func<T> factory) where T : class
    {
        Services[typeof(T)] = new(typeof(T), () => factory()!, null);
    }
}
