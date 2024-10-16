using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.DataAccess.HttpRepository;
public static class HttpRepositoryServices
{
    public static void AddHttpRepositories(this IServiceCollection services)
    {
        services.AddApis();

        services.AddSingleton<IRepository<Member>, EmployeeRepository>();
        services.AddSingleton<IRepository<Order>, OrderRepository>();
        services.AddSingleton<IRepository<Product>, DishesWithCacdedValueRepository>();
        services.AddSingleton<IRepository<Category>, CategoryRepository>();
        services.AddSingleton<IRepository<Ingredient>, IngridientRepository>();
        services.AddSingleton<IRepository<Receipt>, ReceiptRepository>();
        services.AddSingleton<IRepository<SeizureReason>, SeizureRepository>();
        services.AddSingleton<IRepository<ContributionReason>, ContributionReasonRepository>();
        services.AddSingleton<IAdditiveRepository, AdditiveRepository>();
        services.AddSingleton<IRepository<Additive>>(sp => sp.GetRequiredService<IAdditiveRepository>());
        services.AddSingleton<IShiftRepository, ShiftRepository>();
        services.AddSingleton<IRepository<Shift>>(sp => sp.GetRequiredService<IShiftRepository>());

/*
        RcsLocatorBuilder.AddSingleton<IRepository<Member>, EmployeeRepository>();
        RcsLocatorBuilder.AddSingleton<IRepository<Order>, OrderRepository>();
        RcsLocatorBuilder.AddSingleton<IRepository<Product>, DishesWithCacdedValueRepository>();
        RcsLocatorBuilder.AddSingleton<IRepository<Category>, CategoryRepository>();
        RcsLocatorBuilder.AddSingleton<IRepository<Ingredient>, IngridientRepository>();
        RcsLocatorBuilder.AddSingleton<IRepository<Receipt>, ReceiptRepository>();
        RcsLocatorBuilder.AddSingleton<IRepository<SeizureReason>, SeizureRepository>();
        RcsLocatorBuilder.AddSingleton<IRepository<ContributionReason>, ContributionReasonRepository>();

        var additiveRepository = new AdditiveRepository();

        RcsLocatorBuilder.AddSingleton<IAdditiveRepository>(additiveRepository);
        RcsLocatorBuilder.AddSingleton<IRepository<Additive>>(additiveRepository);

        var shiftRepository = new ShiftRepository();

        RcsLocatorBuilder.AddSingleton<IShiftRepository>(shiftRepository);
        RcsLocatorBuilder.AddSingleton<IRepository<Shift>>(shiftRepository);

        RcsLocatorBuilder.AddToBuilder();
*/
    }

    public static void AddApis(this IServiceCollection services)
    {
        services.AddApi<IEmployeeApi>();
        services.AddApi<IOrdersApi>();
        services.AddApi<IDishesApi>();
        services.AddApi<IDishGroupApi>();
        services.AddApi<IIngridientsApi>();
        services.AddApi<ITechcardApi>();
        services.AddApi<IAdditiveApi>();
        services.AddApi<IEmployeePostApi>();
        services.AddApi<IFundApi>();
    }
}
