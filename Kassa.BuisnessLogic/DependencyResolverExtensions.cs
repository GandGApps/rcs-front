using Splat;

namespace Kassa.BuisnessLogic;

public static class DependencyResolverExtensions
{
    public static void RegisterBuisnessLogic(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<ICashierService>(() => new CashierService());
    }

    public static T GetRequiredService<T>(this IReadonlyDependencyResolver services)
    {
        return services.GetService<T>() ?? throw new InvalidOperationException($"The service of type {typeof(T)} is not registered.");
    }
}
