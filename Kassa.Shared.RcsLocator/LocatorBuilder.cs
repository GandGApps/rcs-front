using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.Locator;

/// <summary>
/// Using aggresive optimizations to make the generated code as fast as possible.
/// </summary>
public sealed class LocatorBuilder
{

    internal readonly Dictionary<Type, ServiceDesciptor> Services = [];
    internal readonly Dictionary<Type, ServiceDesciptor> ScopedServices = [];

    /// <summary>
    /// Do not call this method directly. This method is called by the generated code.
    /// </summary>
    public unsafe void AddService<TService, TImpelementation>(delegate* managed<object> factory) 
        where TService : class
        where TImpelementation : TService
    {
        Services.Add(typeof(TService), new ServiceDesciptor(typeof(TImpelementation), factory));
    }

    /// <summary>
    /// Do not call this method directly. This method is called by the generated code. 
    /// The <see cref="RcsScopedLocator"/> uses the factory each time a new scope is created, and the factory must return a new instance of the service.
    /// </summary>
    public unsafe void AddScopedService<TService, TImpelementation>(delegate* managed<object> factory) 
        where TService : IInitializable
        where TImpelementation : TService
    {
        ScopedServices.Add(typeof(TService), new ServiceDesciptor(typeof(TImpelementation), factory));
    }

    public RcsLocator Build()
    {
        var services = Services.ToFrozenDictionary();
        var scopedServices = ScopedServices.ToFrozenDictionary();
        return new(services, scopedServices);
    }
}