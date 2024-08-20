﻿using Kassa.BuisnessLogic.Edgar.Services;
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
    [Obsolete("Use BuisnessLogicEdgarServices instead")]
    public static void AddEdgarBuisnessLogic(this IMutableDependencyResolver services)
    {

        services.RegisterConstant<IAuthService>(new AuthService());

        services.Register<IShiftService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Shift>>();
            var memberService = Locator.Current.GetNotInitializedService<IMemberService>();
            var authService = Locator.Current.GetRequiredService<IAuthService>();

            return new ShiftService(repository, memberService, authService);
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
            var shiftService = Locator.Current.GetNotInitializedService<IShiftService>();

            return new CashierService(additiveService, categoryService, productService, receiptService, ordersService, paymentInfoService, shiftService);
        });
        services.RegisterInitializableServiceFactory<ICashierService>();

        services.Register<IAdditiveService>(() =>
        {

            var repository = Locator.Current.GetRequiredService<IAdditiveRepository>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();

            return new AdditiveService(repository, receiptService);
        });

        services.RegisterConstant<IReportShiftService>(new ReportShiftService());

        services.Register<IOrdersService>(() =>
        {
            var orderRepository = Locator.Current.GetRequiredService<IRepository<Order>>();
            var paymentInfoRepository = Locator.Current.GetRequiredService<IRepository<PaymentInfo>>();

            return new OrdersService(orderRepository, paymentInfoRepository);
        });
        services.RegisterInitializableServiceFactory<IOrdersService>();

        services.Register<IIngridientsService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Ingredient>>();

            return new IngridientsService(repository);
        });
        services.RegisterInitializableServiceFactory<IIngridientsService>();

        SplatRegistrations.SetupIOC();
    }

}
