using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.Shared.ServiceLocator;
public readonly struct ServiceScope: IServiceProvider, IAsyncDisposable, IDisposable, IEnableLogger
{

    public static ServiceScope Empty
    {
        get;
    } = new ServiceScope(FrozenDictionary<Type, object>.Empty);

    private readonly FrozenDictionary<Type, object> _services;

    private ServiceScope(FrozenDictionary<Type, object> services)
    {
        _services = services;
    }

    public readonly bool IsEmpty => _services.Count == 0;

    public static async ValueTask<ServiceScope> CreateScope(List<(Type, Func<IServiceProvider,object>)> scopedServices)
    {
        var services = new Dictionary<Type, object>();
        var tempServiceProvider = new TempServiceProvider(services, scopedServices);

        foreach (var (type, factory) in scopedServices)
        {
            if (services.ContainsKey(type))
            {
                continue;
            }

            var service = factory(tempServiceProvider);

            if (service is IInitializable initializable)
            {
                await initializable.Initialize();
            }

            var needToInits = tempServiceProvider.GetNeedToInits();

            foreach (var needToInit in needToInits)
            {
                await needToInit.Initialize();
            }

            services[type] = service;
        }

        return new(services.ToFrozenDictionary());
    }

    public readonly object? GetService(Type serviceType)
    {
        if (_services.TryGetValue(serviceType, out var service))
        {
            return service;
        }

        return null;
    }

    public readonly T? GetService<T>() where T : class
    {
        var service = GetService(typeof(T));

        if (service == null)
        {
            return null;
        }

        // This is safe because we know that the service is of type T
        return Unsafe.As<T>(service);
    }

    public readonly T GetRequiredService<T>() where T : class
    {
        var service = GetService<T>();

        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        }

        return service;
    }

    public readonly void Dispose()
    {
        if (IsEmpty)
        {
            return;
        }

        foreach (var service in _services.Values)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public async readonly ValueTask DisposeAsync()
    {
        if (IsEmpty)
        {
            return;
        }

        foreach (var service in _services.Values)
        {
            try
            {
                if (service is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
            }
            catch (Exception exc)
            {
                // Log exception
                this.Log().Error(exc, $"Error disposing service {service.GetType()}");
            }
            
        }
    }

    private sealed class TempServiceProvider(Dictionary<Type, object> services, List<(Type, Func<IServiceProvider, object>)> scopedServices) : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = services;
        private readonly List<(Type, Func<IServiceProvider, object>)> _scopedServices = scopedServices;
        private readonly List<IInitializable> _needToInit = [];

        public object? GetService(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var value))
            {
                return value;
            }

            if(_scopedServices.Any(x => x.Item1 == serviceType))
            {
                _services[serviceType] = _scopedServices.First(x => x.Item1 == serviceType).Item2(this);

                if (_services[serviceType] is IInitializable initializable)
                {
                    _needToInit.Add(initializable);
                }

                return _services[serviceType];
            }

            return RcsLocator.GetService(serviceType);
        }

        public IInitializable[] GetNeedToInits()
        {
            var result = _needToInit.ToArray();

            _needToInit.Clear();

            return result;
        }
    }
}
