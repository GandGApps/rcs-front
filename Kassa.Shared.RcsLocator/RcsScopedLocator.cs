using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Splat;

namespace Kassa.Shared.Locator;
public sealed class RcsScopedLocator : IServiceProvider, IAsyncDisposable, IDisposable, IEnableLogger
{
    private readonly FrozenDictionary<Type, ServiceDesciptor> _services = null!;
    private FrozenDictionary<Type, object>? _scope = null;

    internal RcsScopedLocator(FrozenDictionary<Type, ServiceDesciptor> services)
    {
        _services = services;
    }

    public object? GetService(Type serviceType) => _services[serviceType].GetService();

    public T? GetService<T>() where T : class, IInitializable => _services[typeof(T)].GetService<T>();

    public T GetRequiredService<T>() where T : class, IInitializable => _services[typeof(T)].GetRequiredService<T>();

    public async ValueTask CreateScope()
    {
        if (_scope is not null)
        {
            ThrowHelper.ThrowInvalidOperationException("Scope already created.");
        }

        var scope = new Dictionary<Type, object>(_services.Count);

        foreach (var serviceDescriptor in _services.Values)
        {
            var service = serviceDescriptor.GetService();
            var initializable = Unsafe.As<IInitializable>(service);

            if (initializable is not null)
            {
                await initializable.Initialize();

                scope.Add(serviceDescriptor.ServiceType, initializable);
            }
        }

        _scope = scope.ToFrozenDictionary();
    }

    /// <summary>
    /// Prefer calling this method instead of <see cref="Dispose"/>. 
    /// </summary>
    /// <remarks>
    /// Disposing scope will dispose all services that implement <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// But you can still use this object to create a new scope.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        if (_scope is null)
        {
            this.Log().Warn("Scope is not created. Nothing to dispose.");
            return;
        }

        try
        {
            foreach (var service in _scope.Values)
            {
                if (service is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        finally
        {
            _scope = null;
        }
    }

    /// <summary>
    /// Prefer calling <see cref="DisposeAsync"/> instead of this method.
    /// </summary>
    /// /// <remarks>
    /// Disposing scope will dispose all services that implement <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// But you can still use this object to create a new scope.
    /// </remarks>
    public void Dispose()
    {
        if (_scope is null)
        {
            this.Log().Warn("Scope is not created. Nothing to dispose.");
            return;
        }

        try
        {
            foreach (var service in _scope.Values)
            {
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        finally
        {
            _scope = null;
        }
    }
}