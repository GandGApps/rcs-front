using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.BuisnessLogic.Edgar;
public static class BuisnessLogicEdgarServices
{
    public static void AddEdgarBuisnessLogic(this IServiceCollection services)
    {
        services.AddApis();

        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IShiftService, ShiftService>();
        services.AddSingleton<IReportShiftService, ReportShiftService>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICashierService, CashierService>();
        services.AddScoped<IAdditiveService, AdditiveService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IIngridientsService, IngridientsService>();
        services.AddScoped<IContributionReasonService, ContributionReasonService>();
        services.AddScoped<ISeizureReasonService, SeizureReasonService>();
        services.AddScoped<IOrdersService, OrdersService>();

/*
        RcsLocatorBuilder.AddSingleton<IAuthService, AuthService>();
        RcsLocatorBuilder.AddSingleton<IShiftService, ShiftService>();
        RcsLocatorBuilder.AddSingleton<IReportShiftService, ReportShiftService>();

        RcsLocatorBuilder.AddScoped<IProductService, ProductService>();
        RcsLocatorBuilder.AddScoped<ICashierService, CashierService>();
        RcsLocatorBuilder.AddScoped<IAdditiveService, AdditiveService>();
        RcsLocatorBuilder.AddScoped<ICategoryService, CategoryService>();
        RcsLocatorBuilder.AddScoped<IIngridientsService, IngridientsService>();
        RcsLocatorBuilder.AddScoped<IContributionReasonService, ContributionReasonService>();
        RcsLocatorBuilder.AddScoped<ISeizureReasonService, SeizureReasonService>();
        RcsLocatorBuilder.AddScoped<IOrdersService, OrdersService>();

        RcsLocatorBuilder.AddToBuilder();
*/
    }

    private static void AddApis(this IServiceCollection services)
    {
        services.AddApi<ITerminalApi>();
        services.AddApi<IEmployeePostsApi>();
        services.AddApi<ITerminalPostApi>();
        services.AddApi<IEmployeeApi>();
    }
}
