using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared.ServiceLocator;

namespace Kassa.DataAccess;
public static class DataAccessMockServices
{

    public static void RegisterMock()
    {
        RcsLocatorBuilder.AddSingleton<IRepository<Product>>(IRepository<Product>.CreateMock("MockProducts.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<Category>>(IRepository<Category>.CreateMock("MockCategories.json"));
        RcsLocatorBuilder.AddSingleton<IAdditiveRepository>(
            new IAdditiveRepository.MockAdditveRepository(
                IRepository<Additive>.CreateMock("MockAdditive.json")
            )
        );
        RcsLocatorBuilder.AddSingleton<IRepository<Client>>(IRepository<Client>.CreateMock("MockClient.json"));

        var streets = IRepository<Street>.CreateMock("MockStreets.json");
        var shifts = IRepository<Shift>.CreateMock("MockShifts.json");

        RcsLocatorBuilder.AddSingleton<IRepository<Street>>(streets);
        RcsLocatorBuilder.AddSingleton<IDistrictRepository>(IDistrictRepository.CreateMock("MockDistricts.json", streets));
        RcsLocatorBuilder.AddSingleton<IRepository<Order>>(new IRepository<Order>.MockRepository([]));
        RcsLocatorBuilder.AddSingleton<IRepository<PaymentInfo>>(new IRepository<PaymentInfo>.MockRepository([]));
        RcsLocatorBuilder.AddSingleton<IStreetRepository>(IStreetRepository.CreateStreetMock("MockStreets.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<Ingredient>>(IRepository<Ingredient>.CreateMock("MockIngredients.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<Receipt>>(IRepository<Receipt>.CreateMock("MockReceipts.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<Courier>>(IRepository<Courier>.CreateMock("MockCouriers.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<Shift>>(shifts);
        RcsLocatorBuilder.AddSingleton<IShiftRepository>(new IShiftRepository.MockShiftRepository(shifts));
        RcsLocatorBuilder.AddSingleton<IRepository<User>>(IRepository<User>.CreateMock("MockUsers.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<SeizureReason>>(IRepository<SeizureReason>.CreateMock("MockWithdrawalReason.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<ContributionReason>>(IRepository<ContributionReason>.CreateMock("MockWithdrawalReason.json"));
        RcsLocatorBuilder.AddSingleton<IRepository<Member>>(IRepository<Member>.CreateMock("MockMembers.json"));

        RcsLocatorBuilder.AddToBuilder();
    }

}
