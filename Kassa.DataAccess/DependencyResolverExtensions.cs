using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Splat;

namespace Kassa.DataAccess;
public static class DependencyResolverExtensions
{
    public static void RegisterMockDataAccess(this IMutableDependencyResolver services)
    {
        services.RegisterConstant(IRepository<Product>.CreateMock("MockProducts.json"));
        services.RegisterConstant(IRepository<Category>.CreateMock("MockCategories.json"));
        services.RegisterConstant<IAdditiveRepository>(
            new IAdditiveRepository.MockAdditveRepository(
                IRepository<Additive>.CreateMock("MockAdditive.json")
            )
        );
        services.RegisterConstant(IRepository<Client>.CreateMock("MockClient.json"));

        var streets = IRepository<Street>.CreateMock("MockStreets.json");
        var shifts = IRepository<Shift>.CreateMock("MockShifts.json");

        services.RegisterConstant(streets);
        services.RegisterConstant(IDistrictRepository.CreateMock("MockDistricts.json", streets));
        services.RegisterConstant<IRepository<Order>>(new IRepository<Order>.MockRepository([]));
        services.RegisterConstant<IRepository<PaymentInfo>>(new IRepository<PaymentInfo>.MockRepository([]));
        services.RegisterConstant(streets);
        services.RegisterConstant(IStreetRepository.CreateStreetMock("MockStreets.json"));
        services.RegisterConstant(IRepository<Ingredient>.CreateMock("MockIngredients.json"));
        services.RegisterConstant(IRepository<Receipt>.CreateMock("MockReceipts.json"));
        services.RegisterConstant(IRepository<Courier>.CreateMock("MockCouriers.json"));
        services.RegisterConstant(shifts);
        services.RegisterConstant<IShiftRepository>(new IShiftRepository.MockShiftRepository(shifts));
        services.RegisterConstant(IRepository<User>.CreateMock("MockUsers.json"));
        services.RegisterConstant(IRepository<WithdrawalReason>.CreateMock("MockWithdrawalReason.json"));
        services.RegisterConstant(IRepository<DepositReason>.CreateMock("MockWithdrawalReason.json"));
        services.RegisterConstant(IRepository<Member>.CreateMock("MockMembers.json"));
    }
}
