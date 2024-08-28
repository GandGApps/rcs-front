using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;

namespace Kassa.DataAccess.HttpRepository;
public static class HttpRepositoryServices
{
    public static void RegisterServices()
    {
        AddApis();

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
    }

    public static void AddApis()
    {
        ApiServiceRegistration.AddApi<IEmployeeApi>();
        ApiServiceRegistration.AddApi<IOrdersApi>();
        ApiServiceRegistration.AddApi<IDishesApi>();
        ApiServiceRegistration.AddApi<IDishGroupApi>();
        ApiServiceRegistration.AddApi<IIngridientsApi>();
        ApiServiceRegistration.AddApi<ITechcardApi>();
        ApiServiceRegistration.AddApi<IAdditiveApi>();
        ApiServiceRegistration.AddApi<IEmployeePostApi>();
        ApiServiceRegistration.AddApi<IFundApi>();
    }
}
