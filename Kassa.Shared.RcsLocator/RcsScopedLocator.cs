using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.RcsLocator;
public sealed class RcsScopedLocator : IServiceProvider, IAsyncDisposable, IDisposable
{
    private FrozenDictionary<Type, ServiceDesciptor> _services = null!;

    public object? GetService(Type serviceType) => _services[serviceType].GetService();

    public T? GetService<T>() where T : class, IInitializable => _services[typeof(T)].GetService<T>();

    public T GetRequiredService<T>() where T : class, IInitializable => _services[typeof(T)].GetRequiredService<T>();

    public async ValueTask CreateScope()
    {
        foreach (var service in _services.Values)
        {
            service.CreateInstance();

            var initializer = Unsafe.As<IInitializable>(service.Instance)!;

            if (!initializer.IsInitialized)
            {
                await initializer.Initialize();
            }
        }
    }

    /// <summary>
    /// Prefer calling this method instead of Dispose.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        foreach (var service in _services.Values)
        {
            if (service.Instance is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }

            service.Instance = null!;
        }
    }

    /// <summary>
    /// Prefer calling <see cref="DisposeAsync"/> instead of this method.
    /// </summary>
    public void Dispose()
    {
        foreach (var service in _services.Values)
        {
            if (service.Instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    internal void SetServices(FrozenDictionary<Type, ServiceDesciptor> services)
    {
        _services = services;
    }
}