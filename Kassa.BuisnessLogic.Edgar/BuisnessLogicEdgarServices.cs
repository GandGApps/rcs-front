using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;

namespace Kassa.BuisnessLogic.Edgar;
public static class BuisnessLogicEdgarServices
{
    public static void RegisterBuisnessLogic()
    {
        AddApis();

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
    }

    private static void AddApis()
    {
        RcsKassa.AddContext(BlEdgarJsonSerializerContext.Default);

        ApiServiceRegistration.AddApi<ITerminalApi>();
        ApiServiceRegistration.AddApi<IEmployeePostsApi>();
        ApiServiceRegistration.AddApi<ITerminalPostApi>();
        ApiServiceRegistration.AddApi<IEmployeeApi>();
    }
}
