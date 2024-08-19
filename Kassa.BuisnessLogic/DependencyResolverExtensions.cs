using System.Runtime.InteropServices;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Kassa.Shared.Locator;
using Splat;

namespace Kassa.BuisnessLogic;

public static class DependencyResolverExtensions
{
    [Obsolete("Remove asap")]
    /// <summary>
    /// Registers business logic services in the dependency resolver.
    /// </summary>
    /// <param name="services">The mutable dependency resolver to which the services will be registered.</param>
    public static void RegisterMockBuisnessLogic(this IMutableDependencyResolver services)
    {
        SplatRegistrations.SetupIOC();

        SplatRegistrations.Register<ICategoryService, CategoryService>();
        RegisterInitializableServiceFactory<ICategoryService>(services);

        services.Register<IProductService>(() =>
        {

            var repository = Locator.Current.GetRequiredService<IRepository<Product>>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();

            return new ProductService(repository, receiptService);
        });
        RegisterInitializableServiceFactory<IProductService>(services);

        services.Register<IAdditiveService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IAdditiveRepository>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();

            return new AdditiveService(repository, receiptService);
        });
        RegisterInitializableServiceFactory<IAdditiveService>(services);

        SplatRegistrations.Register<IShiftService, ShiftService>();
        RegisterInitializableServiceFactory<IShiftService>(services);

        SplatRegistrations.Register<IUserService, UserService>();
        RegisterInitializableServiceFactory<IUserService>(services);

        SplatRegistrations.Register<IClientService, ClientService>();
        RegisterInitializableServiceFactory<IClientService>(services);

        SplatRegistrations.Register<IPaymentInfoService, PaymentInfoService>();
        RegisterInitializableServiceFactory<IPaymentInfoService>(services);

        SplatRegistrations.Register<IStreetService, StreetService>();
        RegisterInitializableServiceFactory<IStreetService>(services);

        SplatRegistrations.Register<IDistrictService, DistrictService>();
        RegisterInitializableServiceFactory<IDistrictService>(services);

        /*SplatRegistrations.Register<IIngridientsService, IngridientsService>();
        RegisterInitializableServiceFactory<IIngridientsService>(services);*/

        SplatRegistrations.Register<ICourierService, CourierService>();
        RegisterInitializableServiceFactory<ICourierService>(services);

        /*SplatRegistrations.Register<IOrdersService, OrdersService>();
        RegisterInitializableServiceFactory<IOrdersService>(services);*/

        SplatRegistrations.Register<IWithdrawReasonService, WithdrawReasounService>();
        RegisterInitializableServiceFactory<IWithdrawReasonService>(services);

        SplatRegistrations.Register<IMemberService, MemberService>();
        RegisterInitializableServiceFactory<IMemberService>(services);

        SplatRegistrations.Register<IDepositReasonService, DepositReasonService>();
        RegisterInitializableServiceFactory<IDepositReasonService>(services);

        services.Register<IReceiptService>(() =>
        {
            var ingridientsService = Locator.Current.GetNotInitializedService<IIngridientsService>();
            var repository = Locator.Current.GetRequiredService<IRepository<Receipt>>();

            return new ReceiptService(repository, ingridientsService);
        });
        RegisterInitializableServiceFactory<IReceiptService>(services);
    }

    /// <summary>
    /// Registers a factory for creating initializable services. This factory is responsible for 
    /// the lifecycle management of services that implement IInitializableService, including their 
    /// initialization and disposal.
    /// </summary>
    /// <typeparam name="T">The type of the initializable service to register.</typeparam>
    /// <param name="services">The mutable dependency resolver for service registration.</param>
    /// <remarks>
    /// This method is crucial for services that require explicit initialization steps 
    /// before being used, as defined in the IInitializableService interface. The registered 
    /// factory ensures that these initialization steps are performed as needed.
    /// </remarks>
    public static void RegisterInitializableServiceFactory<T>(this IMutableDependencyResolver services) where T : class, IInitializableService
    {
        services.RegisterConstant<IInitializableServiceFactory<T>>(new InitializableServiceFactory<T>());
    }

    

    /// <summary>
    /// Asynchronously retrieves an initialized service of a specified type. The service is initialized
    /// if it has not been initialized before. This is useful for services that need to perform 
    /// some setup before they are used for the first time.
    /// </summary>
    /// <typeparam name="T">The type of the initializable service to retrieve and initialize.</typeparam>
    /// <param name="services">The read-only dependency resolver from which to retrieve the service.</param>
    /// <returns>A value task containing the initialized service of type T.</returns>
    /// <remarks>
    /// The initialization process is managed by the <see cref="IInitializableServiceFactory{T}"/> registered for T.
    /// This ensures that the service is properly prepared and ready for use, following the 
    /// initialization logic defined in <see cref="IInitializableService"/>.
    /// </remarks>
    public static ValueTask<T> GetInitializedService<T>(this IReadonlyDependencyResolver services) where T : class, IInitializableService
    {
        var serviceFactory = services.GetRequiredService<IInitializableServiceFactory<T>>();

        return serviceFactory.GetService();
    }

    /// <summary>
    /// Retrieves a service of a specified type without explicitly initializing it. However, this method does not guarantee
    /// that the service has not been previously initialized within the same scope. It only ensures that the service is within 
    /// scope and has not been disposed.
    /// </summary>
    /// <typeparam name="T">The type of the initializable service to retrieve.</typeparam>
    /// <param name="services">The read-only dependency resolver from which to retrieve the service.</param>
    /// <returns>The requested service of type T, which might already be initialized if done so earlier in the scope.</returns>
    /// <remarks>
    /// It's important to note that if the service has been initialized in the scope (either manually or via <see cref="GetInitializedService{T}"/>),
    /// <see cref="GetNotInitializedService{T}"/> will still return the initialized instance. To create a new, non-initialized, and non-scoped service,
    /// use <see cref="GetRequiredService{T}"/> instead.
    /// </remarks>
    public static T GetNotInitializedService<T>(this IReadonlyDependencyResolver services) where T : class, IInitializableService
    {
        var serviceFactory = services.GetRequiredService<IInitializableServiceFactory<T>>();

        return serviceFactory.GetNotInitializedService();
    }
}
