using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Refit;
using Kassa.BuisnessLogic.Edgar.Api;
using Splat.Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Kassa.Shared.DelegatingHandlers;
using Kassa.Shared;
using Kassa.DataAccess.Repositories;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Edgar;

public static class EdgarDependencyResolverExntesions
{

    public static void AddEdgarBuisnessLogic(this IMutableDependencyResolver services)
    {
        AddApis(services);

        services.RegisterConstant<IAuthService>(new AuthService());

        services.Register<IShiftService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Shift>>();
            var memberService = Locator.Current.GetNotInitializedService<IMemberService>();

            return new ShiftService(repository, memberService);
        });
        services.RegisterInitializableServiceFactory<IShiftService>();

        services.Register<IProductService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Product>>();
            var ingridientsService = Locator.Current.GetRequiredService<IIngridientsService>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();

            return new ProductService(repository, ingridientsService, receiptService);
        });

        services.Register<ICashierService>(() =>
        {
            var additiveService = Locator.Current.GetNotInitializedService<IAdditiveService>();
            var categoryService = Locator.Current.GetNotInitializedService<ICategoryService>();
            var productService = Locator.Current.GetNotInitializedService<IProductService>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();
            var ordersService = Locator.Current.GetNotInitializedService<IOrdersService>();
            var paymentInfoService = Locator.Current.GetNotInitializedService<IPaymentInfoService>();

            return new CashierService(additiveService, categoryService, productService, receiptService, ordersService, paymentInfoService);
        });
        services.RegisterInitializableServiceFactory<ICashierService>();

        SplatRegistrations.SetupIOC();
    }

    internal static void AddApis(IMutableDependencyResolver services)
    {
        services.AddApi<ITerminalApi>();
        services.AddApi<IEmployeePostsApi>();
        services.AddApi<IEmployeeApi>();
    }
}
