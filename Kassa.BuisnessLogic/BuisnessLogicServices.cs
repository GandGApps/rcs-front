using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.BuisnessLogic;
public static class BuisnessLogicServices
{
    public static void AddMockBuisnessLogic(this IServiceCollection services)
    {
        services.AddScoped<IReceiptService, ReceiptService>();
        //services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IPaymentInfoService, PaymentInfoService>();
        services.AddScoped<IStreetService, StreetService>();
        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<ICourierService, CourierService>();

        services.AddSingleton<IMemberService, MemberService>();
/*
        RcsLocatorBuilder.AddScoped<IReceiptService, ReceiptService>();
        //RcsLocatorBuilder.AddScoped<IUserService, UserService>();
        RcsLocatorBuilder.AddScoped<IClientService, ClientService>();
        RcsLocatorBuilder.AddScoped<IPaymentInfoService, PaymentInfoService>();
        RcsLocatorBuilder.AddScoped<IStreetService, StreetService>();
        RcsLocatorBuilder.AddScoped<IDistrictService, DistrictService>();
        RcsLocatorBuilder.AddScoped<ICourierService, CourierService>();
        

        RcsLocatorBuilder.AddSingleton<IMemberService, MemberService>();

        RcsLocatorBuilder.AddToBuilder();
*/
    }
}
