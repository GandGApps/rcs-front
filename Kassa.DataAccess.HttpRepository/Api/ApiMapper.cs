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

    public static Order MapEdgarModelToOrder(OrderEdgarModel edgarModel)
    {
        return new Order
        {
            Id = edgarModel.Id,
            Problem = edgarModel.Problem,
            Status = Enum.Parse<OrderStatus>(edgarModel.Status),
            CreatedAt = edgarModel.CreatedAt,
            DeliveryTime = edgarModel.DeliveryTime,
            CourierId = edgarModel.CourierId,
            Products = edgarModel.Products.Select(MapEdgarModelToOrderedProduct).ToList(),
            Comment = edgarModel.Comment,
            TotalSum = edgarModel.TotalSum,
            SubtotalSum = edgarModel.SubTotalSum,
            Discount = edgarModel.Discount,
            IsDelivery = edgarModel.IsDelivery,
            ClientId = edgarModel.ClientId,
            LastName = edgarModel.LastName,
            Phone = edgarModel.Phone,
            Card = edgarModel.Card,
            Miscellaneous = edgarModel.Miscellaneous,
            House = edgarModel.House,
            Building = edgarModel.Building,
            Entrance = edgarModel.Entrance,
            Floor = edgarModel.Floor,
            Apartment = edgarModel.Apartment,
            Intercom = edgarModel.Intercom,
            AddressNote = edgarModel.AddressNote,
            IsPickup = edgarModel.IsPickup,
            StreetId = edgarModel.StreetId,
            DistrictId = edgarModel.DistrictId,
            FirstName = edgarModel.FirstName,
            MiddleName = edgarModel.MiddleName,
            IsOutOfTurn = edgarModel.IsOutOfTurn,
            IsProblematicDelivery = edgarModel.IsProblematicDelivery,
            PaymentInfo = new PaymentInfo
            {
                Cash = edgarModel.PayInfCash,
                BankСard = edgarModel.PayInfBankCart,
                CashlessPayment = edgarModel.PayInfCashless,
                WithoutRevenue = edgarModel.PayInfWithoutRev,
                ToDeposit = edgarModel.PayInfToDeposit,
                ToEntered = edgarModel.PayinfToEntered,
                Change = edgarModel.PayInfChange,
                WithSalesReceipt = edgarModel.PayInfWithSalesReceipt
            },
            PaymentInfoId = null // Adjust as necessary
        };
    }

    public static OrderedProduct MapEdgarModelToOrderedProduct(OrderedProductEdgarModel edgarModel)
    {
        return new OrderedProduct
        {
            Id = edgarModel.Id,
            ProductId = edgarModel.ProductId,
            Count = edgarModel.Count,
            Price = edgarModel.Price,
            TotalPrice = edgarModel.TotalPrice,
            SubTotalPrice = edgarModel.SubTotalPrice,
            Discount = edgarModel.Discount,
            Comment = edgarModel.Comment,
            Additives = edgarModel.Additives.Select(MapEdgarModelToOrderedAdditive).ToList()
        };
    }

    public static OrderedAdditive MapEdgarModelToOrderedAdditive(OrderedAdditiveEdgarModel edgarModel)
    {
        return new OrderedAdditive
        {
            Id = edgarModel.Id,
            AdditiveId = edgarModel.Id,
            Count = edgarModel.Count,
            Price = edgarModel.Price,
            TotalPrice = edgarModel.TotalPrice,
            SubtotalPrice = edgarModel.SubTotalPrice,
            Discount = edgarModel.Discount,
            Measure = edgarModel.Measure
        };
    }

    public static PaymentInfo MapEdgarModelToPaymentInfo(PaymentInfoEdgarModel edgarModel)
    {
        return new PaymentInfo
        {
            Id = edgarModel.Id,
            OrderId = edgarModel.OrderId,
            Cash = edgarModel.Cash,
            BankСard = edgarModel.BankCard,
            CashlessPayment = edgarModel.CashlessPayment,
            WithoutRevenue = edgarModel.WithoutRevenue,
            ToDeposit = edgarModel.ToDeposit,
            ToEntered = edgarModel.ToEntered,
            Change = edgarModel.Change,
            WithSalesReceipt = edgarModel.WithSalesReceipt
        };
    }

    public static OrderEdgarModel MapOrderToEdgarModel(Order order)
    {
        return new OrderEdgarModel
        {
            Id = order.Id,
            Problem = order.Problem,
            IsProblematicDelivery = order.IsProblematicDelivery,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            DeliveryTime = order.DeliveryTime,
            CourierId = order.CourierId,
            Products = order.Products.Select(MapOrderedProductToEdgarModel).ToList(),
            Comment = order.Comment,
            TotalSum = order.TotalSum,
            SubTotalSum = order.SubtotalSum,
            Discount = order.Discount,
            IsDelivery = order.IsDelivery,
            ClientId = order.ClientId,
            LastName = order.LastName,
            Phone = order.Phone,
            Card = order.Card,
            Miscellaneous = order.Miscellaneous,
            House = order.House,
            Building = order.Building,
            Entrance = order.Entrance,
            Floor = order.Floor,
            Apartment = order.Apartment,
            Intercom = order.Intercom,
            AddressNote = order.AddressNote,
            IsPickup = order.IsPickup,
            StreetId = order.StreetId,
            DistrictId = order.DistrictId,
            FirstName = order.FirstName,
            MiddleName = order.MiddleName,
            IsOutOfTurn = order.IsOutOfTurn,
            PayInfCash = order.PaymentInfo != null ? order.PaymentInfo.Cash : 0,
            PayInfBankCart = order.PaymentInfo != null ? order.PaymentInfo.BankСard : 0,
            PayInfCashless = order.PaymentInfo != null ? order.PaymentInfo.CashlessPayment : 0,
            PayInfWithoutRev = order.PaymentInfo != null ? order.PaymentInfo.WithoutRevenue : 0,
            PayInfToDeposit = order.PaymentInfo != null ? order.PaymentInfo.ToDeposit : 0,
            PayinfToEntered = order.PaymentInfo != null ? order.PaymentInfo.ToEntered : 0,
            PayInfChange = order.PaymentInfo != null ? order.PaymentInfo.Change : 0,
            PayInfWithSalesReceipt = order.PaymentInfo != null && order.PaymentInfo.WithSalesReceipt,
            PostId = order.CashierShiftId,
            EmployeePostId = order.ShiftId,
        };
    }

    public static OrderedProductEdgarModel MapOrderedProductToEdgarModel(OrderedProduct product)
    {
        return new OrderedProductEdgarModel
        {
            Id = product.Id,
            ProductId = product.ProductId,
            Count = product.Count,
            Price = product.Price,
            TotalPrice = product.TotalPrice,
            SubTotalPrice = product.SubTotalPrice,
            Discount = product.Discount,
            Comment = product.Comment,
            Additives = product.Additives.Select(MapOrderedAdditiveToEdgarModel).ToList()
        };
    }

    public static OrderedAdditiveEdgarModel MapOrderedAdditiveToEdgarModel(OrderedAdditive additive)
    {
        return new OrderedAdditiveEdgarModel
        {
            Id = additive.Id,
            AdditiveId = additive.AdditiveId,
            Count = additive.Count,
            Price = additive.Price,
            TotalPrice = additive.TotalPrice,
            SubTotalPrice = additive.SubtotalPrice,
            Discount = additive.Discount,
            Measure = additive.Measure
        };
    }

    public static PaymentInfoEdgarModel MapPaymentInfoToEdgarModel(PaymentInfo paymentInfo)
    {
        return new PaymentInfoEdgarModel
        {
            Id = paymentInfo.Id,
            OrderId = paymentInfo.OrderId,
            Cash = paymentInfo.Cash,
            BankCard = paymentInfo.BankСard,
            CashlessPayment = paymentInfo.CashlessPayment,
            WithoutRevenue = paymentInfo.WithoutRevenue,
            ToDeposit = paymentInfo.ToDeposit,
            ToEntered = paymentInfo.ToEntered,
            Change = paymentInfo.Change,
            WithSalesReceipt = paymentInfo.WithSalesReceipt
        };
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


    public static Order MapServerResponseToOrder(EdgarOrderInfoResponse serverResponse)
    {
        var orderResponse = serverResponse.Order;
        var productsResponse = serverResponse.OrderedProducts;

        return new Order
        {
            Id = orderResponse.Id,
            Problem = orderResponse.Problem,
            Status = Enum.Parse<OrderStatus>(orderResponse.Status),
            CreatedAt = orderResponse.CreatedAt,
            DeliveryTime = orderResponse.DeliveryTime,
            CourierId = orderResponse.CourierId,
            Products = productsResponse?.Select(MapServerResponseToOrderedProduct).ToList() ?? new List<OrderedProduct>(),
            Comment = orderResponse.Comment,
            TotalSum = orderResponse.TotalSum,
            SubtotalSum = orderResponse.SubTotalSum,
            Discount = orderResponse.Discount,
            IsDelivery = orderResponse.IsDelivery,
            ClientId = orderResponse.ClientId,
            LastName = orderResponse.LastName,
            Phone = orderResponse.Phone,
            Card = orderResponse.Card,
            Miscellaneous = orderResponse.Miscellaneous,
            House = orderResponse.House,
            Building = orderResponse.Building,
            Entrance = orderResponse.Entrance,
            Floor = orderResponse.Floor,
            Apartment = orderResponse.Apartment,
            Intercom = orderResponse.Intercom,
            AddressNote = orderResponse.AddressNote,
            IsPickup = orderResponse.IsPickup,
            StreetId = orderResponse.StreetId,
            DistrictId = orderResponse.DistrictId,
            FirstName = orderResponse.FirstName,
            MiddleName = orderResponse.MiddleName,
            IsOutOfTurn = orderResponse.IsOutOfTurn,
            IsProblematicDelivery = orderResponse.IsProblematicDelivery,
            PaymentInfo = orderResponse.PaymentInfoId.HasValue ? new PaymentInfo { Id = orderResponse.PaymentInfoId.Value } : null,
            PaymentInfoId = orderResponse.PaymentInfoId,
            ShiftId = orderResponse.EmployeePostId,
            CashierShiftId = orderResponse.PostId
        };
    }

    public static OrderedProduct MapServerResponseToOrderedProduct(OrderedProductEdgarResponse productResponse)
    {
        return new OrderedProduct
        {
            Id = productResponse.Id,
            ProductId = productResponse.ProductId,
            Count = productResponse.Count,
            Price = productResponse.Price,
            TotalPrice = productResponse.TotalPrice,
            SubTotalPrice = productResponse.SubTotalPrice,
            Discount = productResponse.Discount ?? 0,
            Comment = productResponse.Comment ?? string.Empty,
            Additives = productResponse.Additives?.Select(MapServerResponseToOrderedAdditive).ToList() ?? new List<OrderedAdditive>()
        };
    }

    public static OrderedAdditive MapServerResponseToOrderedAdditive(OrderedAdditiveEdgarResponse additiveResponse)
    {
        return new OrderedAdditive
        {
            Id = additiveResponse.Id,
            AdditiveId = additiveResponse.Id,
            Count = additiveResponse.Count,
            Price = additiveResponse.Price,
            TotalPrice = additiveResponse.TotalPrice,
            SubtotalPrice = additiveResponse.SubTotalPrice,
            Discount = additiveResponse.Discount ?? 0,
            Measure = additiveResponse.Measure
        };
    }

    public static Receipt MapTechcardToReceipt(TechcardEdgarResponse techcardResponse)
    {
        return new Receipt
        {
            Id = techcardResponse.TechcardId,
            IngredientUsages = techcardResponse.Ingridients is null ? [] : techcardResponse.Ingridients.Select(MapIngridientToIngredientUsage).ToArray()
        };
    }

    public static IngredientUsage MapIngridientToIngredientUsage(IngridientEdgarResponse ingridientResponse)
    {
        return new IngredientUsage
        {
            IngredientId = ingridientResponse.IngridientId,
            Count = ingridientResponse.Netto 
        };
    }

    public static AdditiveEdgarModel MapAdditiveToEdgarModel(Additive additive)
    {
        return new AdditiveEdgarModel
        {
            Id = additive.Id,
            Name = additive.Name,
            NomenclatureType = string.Empty, // Placeholder, update as needed
            AccountingCategory = string.Empty, // Placeholder, update as needed
            ParentGroupId = null, // Placeholder, update as needed
            Article = 0, // Placeholder, update as needed
            Code = 0, // Placeholder, update as needed
            Warehouse = Guid.Empty, // Placeholder, update as needed
            ModificatorValue = 0, // Placeholder, update as needed
            DishId = additive.ProductIds?.Length > 0 ? additive.ProductIds[0] : Guid.Empty,
            TechcardId = additive.ReceiptId,
            OfficeId = Guid.Empty, // Placeholder, update as needed
            CreatedAt = DateTime.UtcNow, // Placeholder, update as needed
            UpdatedAt = DateTime.UtcNow // Placeholder, update as needed
        };
    }

    public static Additive MapAdditiveEdgarToAdditive(AdditiveEdgarModel edgarModel)
    {
        return new Additive
        {
            Id = edgarModel.Id,
            Name = edgarModel.Name,
            CurrencySymbol = string.Empty, // Placeholder, update as needed
            Price = edgarModel.ModificatorValue, // Assuming modificator_value corresponds to price
            Measure = string.Empty, // Placeholder, update as needed
            ProductIds = [edgarModel.DishId], // Assuming dish_id corresponds to ProductIds
            Portion = 0, // Placeholder, update as needed
            IsAvailable = true, // Default value
            IsEnoughIngredients = true, // Default value
            ReceiptId = edgarModel.TechcardId
        };
    }

}
