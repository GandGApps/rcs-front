using Splat;

namespace Kassa.BuisnessLogic;

public static class DependencyResolverExtensions
{
    public static void RegisterBuisnessLogic(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<ICashierService>(() => new CashierService());
    }
}
