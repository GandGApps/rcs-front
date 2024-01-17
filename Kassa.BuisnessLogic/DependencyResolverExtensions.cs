using Splat;

namespace Kassa.BuisnessLogic;

public static class DependencyResolverExtensions
{
    public static void RegisterBuisnessLogic(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<ICashierService>(() =>
        {
            var productService = Locator.Current.GetRequiredService<IProductService>();
            var categoryService = Locator.Current.GetRequiredService<ICategoryService>();

            return new CashierService(productService, categoryService);
        });
    }

    public static T GetRequiredService<T>(this IReadonlyDependencyResolver services)
    {
        return services.GetService<T>() ?? throw new InvalidOperationException($"The service of type {typeof(T)} is not registered.");
    }

    public static async ValueTask<T> GetInitializedService<T>(this IReadonlyDependencyResolver services) where T : IInitializableService
    {
        var service = services.GetRequiredService<T>();

        await service.Initialize();

        return service;
    }
}
