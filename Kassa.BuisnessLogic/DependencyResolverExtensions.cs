using Kassa.DataAccess;
using Splat;

namespace Kassa.BuisnessLogic;

public static class DependencyResolverExtensions
{
    public static void RegisterBuisnessLogic(this IMutableDependencyResolver services)
    {
        services.Register<ICashierService>(() =>
        {
            var productService = Locator.Current.GetRequiredService<IProductService>();
            var categoryService = Locator.Current.GetRequiredService<ICategoryService>();

            return new CashierService(productService, categoryService);
        });

        services.Register<IProductService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Product>>();

            return new ProductService(repository);
        });

        services.Register<ICategoryService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Category>>();

            return new CategoryService(repository);
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
