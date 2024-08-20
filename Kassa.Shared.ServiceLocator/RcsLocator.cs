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
        return _serviceLocator.GetService(serviceType);
    }

    public static T? GetService<T>() where T : class
    {
        return _serviceLocator.GetService<T>();
    }

    public static T GetRequiredService<T>() where T : class
    {
        return _serviceLocator.GetRequiredService<T>();
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
