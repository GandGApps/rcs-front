using System.Runtime.InteropServices;
using Kassa.DataAccess;
using Splat;

namespace Kassa.BuisnessLogic;

public static class DependencyResolverExtensions
{
    public static void RegisterBuisnessLogic(this IMutableDependencyResolver services)
    {
        services.Register<ICashierService>(() =>
        {
            var productService = Locator.Current.GetNotInitializedService<IProductService>();
            var categoryService = Locator.Current.GetNotInitializedService<ICategoryService>();

            return new CashierService(productService, categoryService);
        });

        SplatRegistrations.Register<ICategoryService, CategoryService>();
        RegisterInitializableServiceFactory<ICategoryService>(services);

        SplatRegistrations.Register<IProductService, ProductService>();
        RegisterInitializableServiceFactory<IProductService>(services);
    }

    public static void RegisterInitializableServiceFactory<T>(this IMutableDependencyResolver services) where T : class, IInitializableService
    {
        services.RegisterConstant<IInitializableServiceFactory<T>>(new InitializableServiceFactory<T>());
    }

    public static T GetRequiredService<T>(this IReadonlyDependencyResolver services)
    {
        return services.GetService<T>() ?? throw new InvalidOperationException($"The service of type {typeof(T)} is not registered.");
    }

    public static ValueTask<T> GetInitializedService<T>(this IReadonlyDependencyResolver services) where T : class, IInitializableService
    {
        var serviceFactory = services.GetRequiredService<IInitializableServiceFactory<T>>();

        return serviceFactory.GetService();
    }

    public static T GetNotInitializedService<T>(this IReadonlyDependencyResolver services) where T : class, IInitializableService
    {
        var serviceFactory = services.GetRequiredService<IInitializableServiceFactory<T>>();

        return serviceFactory.GetNotInitializedService();
    }
}
