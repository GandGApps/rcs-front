using System.Collections.Frozen;

namespace Kassa.Shared.ServiceLocator;

/// <summary>
/// It's more optimized verion of Splat.Locator
/// </summary>
public static class RcsLocator
{
    private static FrozenServiceLocator _serviceLocator;
    private static Func<List<(Type, Func<IServiceProvider, object>)>> _scopeFactory = null!;
    private static ServiceScope _currentScope = ServiceScope.Empty;
    
    public static void SetLocator(FrozenDictionary<Type, Func<object>> servicesFactory)
    {
        _serviceLocator = new(servicesFactory);
    }

    public static void SetScopeFactory(Func<List<(Type, Func<IServiceProvider, object>)>> scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public static object? GetService(Type serviceType)
    {
        var service = _serviceLocator.GetService(serviceType);

        if (!_currentScope.IsEmpty && service is null)
        {
            service = _currentScope.GetService(serviceType);
        }

        return service;
    }

    public static T? GetService<T>() where T : class
    {
        var service = _serviceLocator.GetService<T>();

        if (!_currentScope.IsEmpty && service is null)
        {
            service = _currentScope.GetService<T>();
        }

        return service;
    }

    public static T GetRequiredService<T>() where T : class
    {
        var service = GetService<T>();

        if (!_currentScope.IsEmpty && service is null)
        {
            service = _currentScope.GetService<T>();
        }

        return service ?? throw new InvalidOperationException($"Service of type {typeof(T)} not found");
    }

    public static async ValueTask ActivateScope()
    {
        if (_scopeFactory == null)
        {
            return;
        }

        var servicesFactory = _scopeFactory();

        _currentScope = await ServiceScope.CreateScope(servicesFactory);
    }

    public static async ValueTask DisposeScope()
    {
        if (_currentScope.IsEmpty)
        {
            return;
        }

        await _currentScope.DisposeAsync();
        _currentScope = ServiceScope.Empty;
    }

    public static ServiceScope Scoped => _currentScope;
}
