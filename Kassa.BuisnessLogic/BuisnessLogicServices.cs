using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared.ServiceLocator;

namespace Kassa.BuisnessLogic;
public static class BuisnessLogicServices
{
    public static void RegisterMockBuisnessLogic()
    {
        RcsLocatorBuilder.AddScoped<IReceiptService, ReceiptService>();
        RcsLocatorBuilder.AddScoped<ICategoryService, CategoryService>();
        RcsLocatorBuilder.AddScoped<IProductService, ProductService>();
        RcsLocatorBuilder.AddScoped<IAdditiveService, AdditiveService>();
        //RcsLocatorBuilder.AddScoped<IUserService, UserService>();
        RcsLocatorBuilder.AddScoped<IClientService, ClientService>();
        RcsLocatorBuilder.AddScoped<IPaymentInfoService, PaymentInfoService>();
        RcsLocatorBuilder.AddScoped<IStreetService, StreetService>();
        RcsLocatorBuilder.AddScoped<IDistrictService, DistrictService>();
        RcsLocatorBuilder.AddScoped<ICourierService, CourierService>();
        

        RcsLocatorBuilder.AddSingleton<IShiftService, ShiftService>();
        RcsLocatorBuilder.AddSingleton<IMemberService, MemberService>();

        RcsLocatorBuilder.AddToBuilder();
    }
}
