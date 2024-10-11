using System.Runtime.InteropServices;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
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



        /*SplatRegistrations.Register<ICategoryService, CategoryService>();
        RegisterInitializableServiceFactory<ICategoryService>(services);

        services.Register<IProductService>(() =>
        {

            var repository = RcsLocator.GetRequiredService<IRepository<Product>>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();

            return new ProductService(repository, receiptService);
        });
        RegisterInitializableServiceFactory<IProductService>(services);

        services.Register<IAdditiveService>(() =>
        {
            var repository = RcsLocator.GetRequiredService<IAdditiveRepository>();
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

        *//*SplatRegistrations.Register<IIngridientsService, IngridientsService>();
        RegisterInitializableServiceFactory<IIngridientsService>(services);*//*

        SplatRegistrations.Register<ICourierService, CourierService>();
        RegisterInitializableServiceFactory<ICourierService>(services);

        *//*SplatRegistrations.Register<IOrdersService, OrdersService>();
        RegisterInitializableServiceFactory<IOrdersService>(services);*//*

        SplatRegistrations.Register<ISeizureReasonService, WithdrawReasounService>();
        RegisterInitializableServiceFactory<ISeizureReasonService>(services);

        SplatRegistrations.Register<IMemberService, MemberService>();
        RegisterInitializableServiceFactory<IMemberService>(services);

        SplatRegistrations.Register<IContributionReasonService, ContributionReasonService>();
        RegisterInitializableServiceFactory<IContributionReasonService>(services);

        services.Register<IReceiptService>(() =>
        {
            var ingridientsService = Locator.Current.GetNotInitializedService<IIngridientsService>();
            var repository = RcsLocator.GetRequiredService<IRepository<Receipt>>();

            return new ReceiptService(repository, ingridientsService);
        });
        RegisterInitializableServiceFactory<IReceiptService>(services);*/
    }
}
