using Kassa.Shared.DelegatingHandlers;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Splat;
using System.Text.Json;
using Kassa.DataAccess.Repositories;
using Kassa.DataAccess.Model;
using Microsoft.Extensions.Configuration;
using Kassa.DataAccess.HttpRepository.Api;

namespace Kassa.DataAccess.HttpRepository;


public static class HttpRepositoryDependencyResolverExntesions
{
    [Obsolete("Use HttpRepositoryServices instead")]
    public static void AddHttpRepositoryDataAccess(this IMutableDependencyResolver services)
    {
        //AddApis(services);

        services.RegisterConstant<IRepository<Member>>(new EmployeeRepository());
        services.RegisterConstant<IRepository<Order>>(new OrderRepository());
        services.RegisterConstant<IRepository<Product>>(new DishesWithCacdedValueRepository());
        services.RegisterConstant<IRepository<Category>>(new CategoryRepository());
        services.RegisterConstant<IRepository<Ingredient>>(new IngridientRepository());
        services.RegisterConstant<IRepository<Receipt>>(new ReceiptRepository());

        var additiveRepository = new AdditiveRepository();
        
        services.RegisterConstant<IAdditiveRepository>(additiveRepository);
        services.RegisterConstant<IRepository<Additive>>(additiveRepository);

        var shiftRepository = new ShiftRepository();

        services.RegisterConstant<IShiftRepository>(shiftRepository);
        services.RegisterConstant<IRepository<Shift>>(shiftRepository);
    }

    /*internal static void AddApis(IMutableDependencyResolver services)
    {
        services.AddApi<IEmployeeApi>();
        services.AddApi<IOrdersApi>();
        services.AddApi<IDishesApi>();
        services.AddApi<IDishGroupApi>();
        services.AddApi<IIngridientsApi>();
        services.AddApi<ITechcardApi>();
        services.AddApi<IAdditiveApi>();
        services.AddApi<IEmployeePostApi>();
    }*/
}
