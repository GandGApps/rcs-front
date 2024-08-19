using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace Kassa.Shared.Locator;

public sealed class RcsLocator : IServiceProvider
{
    /// <summary>
    /// Use this only after building the locator
    /// </summary>
    public static RcsLocator Current
    {
        get; internal set;
    } = null!;

    /// <summary>
    /// Build only once!;
    /// </summary>
    public static readonly LocatorBuilder Services = new();

    public static RcsScopedLocator Scoped => Current.RcsScopedLocator;

    private readonly FrozenDictionary<Type, ServiceDesciptor> _services;

    public RcsScopedLocator RcsScopedLocator
    {
        get;
    }

    internal RcsLocator(FrozenDictionary<Type, ServiceDesciptor> services, FrozenDictionary<Type, ServiceDesciptor> scopedServices)
    {
        RcsScopedLocator = new RcsScopedLocator(scopedServices);
        _services = services;
    }

    public object? GetService(Type serviceType) => _services[serviceType].GetService();

    public T? GetService<T>() where T : class, IInitializable => _services[typeof(T)].GetService<T>();

    public T GetRequiredService<T>() where T : class, IInitializable => _services[typeof(T)].GetRequiredService<T>();


    public static void BuildLocator()
    {
        Current = Services.Build();
    }
}