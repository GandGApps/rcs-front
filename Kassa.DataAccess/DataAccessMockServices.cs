using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.DataAccess;
public static class DataAccessMockServices
{

    public static void AddMockDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<Product>>(IRepository<Product>.CreateMock("MockProducts.json"));
        services.AddSingleton<IRepository<Category>>(IRepository<Category>.CreateMock("MockCategories.json"));
        services.AddSingleton<IAdditiveRepository>(
            new IAdditiveRepository.MockAdditveRepository(
                IRepository<Additive>.CreateMock("MockAdditive.json")
            )
        );
        services.AddSingleton<IRepository<Additive>>(sp => sp.GetRequiredService<IAdditiveRepository>());
        services.AddSingleton<IRepository<Client>>(IRepository<Client>.CreateMock("MockClient.json"));
        services.AddSingleton<IRepository<Street>>(IRepository<Street>.CreateMock("MockStreets.json"));
        services.AddSingleton<IDistrictRepository>(sp => IDistrictRepository.CreateMock("MockDistricts.json", sp.GetRequiredService<IRepository<Street>>()));
        services.AddSingleton<IRepository<Order>>(new IRepository<Order>.MockRepository([]));
        services.AddSingleton<IRepository<PaymentInfo>>(new IRepository<PaymentInfo>.MockRepository([]));
        services.AddSingleton<IStreetRepository>(sp => IStreetRepository.CreateStreetMock("MockStreets.json"));
        services.AddSingleton<IRepository<Ingredient>>(IRepository<Ingredient>.CreateMock("MockIngredients.json"));
        services.AddSingleton<IRepository<Receipt>>(IRepository<Receipt>.CreateMock("MockReceipts.json"));
        services.AddSingleton<IRepository<Courier>>(IRepository<Courier>.CreateMock("MockCouriers.json"));
        services.AddSingleton<IRepository<Shift>>(IRepository<Shift>.CreateMock("MockShifts.json"));
        services.AddSingleton<IShiftRepository>(sp => new IShiftRepository.MockShiftRepository(sp.GetRequiredService<IRepository<Shift>>()));
        services.AddSingleton<IRepository<User>>(IRepository<User>.CreateMock("MockUsers.json"));
        services.AddSingleton<IRepository<SeizureReason>>(IRepository<SeizureReason>.CreateMock("MockWithdrawalReason.json"));
        services.AddSingleton<IRepository<ContributionReason>>(IRepository<ContributionReason>.CreateMock("MockWithdrawalReason.json"));
        services.AddSingleton<IRepository<Member>>(IRepository<Member>.CreateMock("MockMembers.json"));

/*
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
*/
    }

}
