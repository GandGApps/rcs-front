using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Riok.Mapperly.Abstractions;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess.HttpRepository.Api;

[Mapper]
internal static partial class ApiMapper
{
    public static partial OrderDetails MapOrderToRequest(Order order);
    public static partial Order MapRequestToOrder(OrderDetails orderDetails);

    public static OrderDetails MapFullOrderToRequest(Order order)
    {
        var orderDetails = MapOrderToRequest(order);

        orderDetails.PayInfBankCart = order.PaymentInfo!.BankСard;
        orderDetails.PayInfCash = order.PaymentInfo.Cash;
        orderDetails.PayInfCashless = order.PaymentInfo.CashlessPayment;
        orderDetails.PayInfWithoutRev = order.PaymentInfo.WithoutRevenue;
        orderDetails.PayInfChange = order.PaymentInfo.Change;
        orderDetails.PayInfToDeposit = order.PaymentInfo.ToEntered;
        orderDetails.PayInfWithSalesReceipt = order.PaymentInfo.WithSalesReceipt;
        orderDetails.PayInfToDeposit = order.PaymentInfo.ToDeposit;

        return orderDetails;
    }

    public static Order MapRequestToFullOrder(OrderDetails orderDetails)
    {

        var order = MapRequestToOrder(orderDetails);

        order.PaymentInfo = new PaymentInfo
        {
            Id = Guid.NewGuid(),
            BankСard = orderDetails.PayInfBankCart,
            Cash = orderDetails.PayInfCash,
            CashlessPayment = orderDetails.PayInfCashless,
            WithoutRevenue = orderDetails.PayInfWithoutRev,
            Change = orderDetails.PayInfChange,
            ToEntered = orderDetails.PayInfToDeposit,
            WithSalesReceipt = orderDetails.PayInfWithSalesReceipt,
            ToDeposit = orderDetails.PayInfToDeposit
        };

        return order;
    }

    public static DishRequest MapDishToRequest(Product product)
    {

        return new DishRequest
        {
            DishId = product.Id,
            Title = product.Name,
            FullPrice = product.Price,
            ParentGroupId = product.CategoryId,
        };
    }

    public static Product MapRequestToDish(DishRequest dishRequest)
    {
        var product = new Product
        {
            Id = dishRequest.DishId,
            Name = dishRequest.Title,
            Price = dishRequest.FullPrice,
            CategoryId = dishRequest.ParentGroupId
        };

        return product;
    }

    public static DishGroupRequest MapCategoryToRequest(Category category)
    {
        return new DishGroupRequest
        {
            GroupModelId = category.Id,
            Title = category.Name,
            ParentGroupId = category.CategoryId
        };
    }

    public static Category MapRequestToCategory(DishGroupRequest dishGroupRequest)
    {
        var category = new Category
        {
            Id = dishGroupRequest.GroupModelId,
            Name = dishGroupRequest.Title,
            CategoryId = dishGroupRequest.ParentGroupId
        };

        return category;
    }

    public static IngredientResponse MapIngredientToEdgarModel(Ingredient ingredient)
    {
        return new IngredientResponse
        {
            IngredientsId = ingredient.Id,
            Title = ingredient.Name,
            Warehouse = string.Empty, // Assuming default value as it's not present in Ingredient
            TerminalId = null, // Assuming null as default as it's not present in Ingredient
            OfficeId = null, // Assuming null as default as it's not present in Ingredient
            Left = ingredient.Count,
            Price = 0, // Assuming default value as it's not present in Ingredient
            PackagingUnit = ingredient.Measure,
            AlcoholPercent = 0, // Assuming default value as it's not present in Ingredient
            Code = string.Empty, // Assuming default value as it's not present in Ingredient
            Article = string.Empty // Assuming default value as it's not present in Ingredient
        };
    }

    public static Ingredient MapEdgarModelToIngredient(IngredientResponse ingredientResponse)
    {
        return new Ingredient
        {
            Id = ingredientResponse.IngredientsId,
            Name = ingredientResponse.Title,
            Count = ingredientResponse.Left,
            Measure = ingredientResponse.PackagingUnit
        };
    }

}
