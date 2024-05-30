using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IOrdersApi: IUseMemberToken
{
    [Get("/orders")]
    public Task<IEnumerable<EdgarOrderInfoResponse>> GetOrders();

    [Post("/orders/create")]
    public Task AddOrder(OrderEdgarModel order);

    /*public Task<OrderEdgarModel> GetOrder([AliasAs("order_id")] Guid id);*/
}

internal sealed record OrderedProductEdgarModel
{
    [JsonPropertyName("orderedproduct_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("productId")]
    public Guid ProductId
    {
        get; init;
    }

    [JsonPropertyName("count")]
    public double Count
    {
        get; init;
    }

    [JsonPropertyName("price")]
    public double Price
    {
        get; init;
    }

    [JsonPropertyName("totalPrice")]
    public double TotalPrice
    {
        get; init;
    }

    [JsonPropertyName("subTotalPrice")]
    public double SubTotalPrice
    {
        get; init;
    }

    [JsonPropertyName("discount")]
    public double Discount
    {
        get; init;
    }

    [JsonPropertyName("comment")]
    public string Comment
    {
        get; init;
    }

    [JsonPropertyName("orderAdditive")]
    public IEnumerable<OrderedAdditiveEdgarModel> Additives
    {
        get; init;
    }
}

internal sealed record OrderedAdditiveEdgarModel
{
    [JsonPropertyName("additive_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("count")]
    public double Count
    {
        get; init;
    }

    [JsonPropertyName("price")]
    public double Price
    {
        get; init;
    }

    [JsonPropertyName("totalPrice")]
    public double TotalPrice
    {
        get; init;
    }

    [JsonPropertyName("subTotalPrice")]
    public double SubTotalPrice
    {
        get; init;
    }

    [JsonPropertyName("discount")]
    public double Discount
    {
        get; init;
    }

    [JsonPropertyName("measure")]
    public string Measure
    {
        get; init;
    }

    [JsonPropertyName("additiveId")]
    public Guid AdditiveId
    {
        get; init;
    }
}

internal sealed record PaymentInfoEdgarModel
{
    [JsonPropertyName("paymentinfo_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("order_id")]
    public Guid OrderId
    {
        get; init;
    }

    [JsonPropertyName("cash")]
    public double Cash
    {
        get; init;
    }

    [JsonPropertyName("bankCard")]
    public double BankCard
    {
        get; init;
    }

    [JsonPropertyName("cashlessPayment")]
    public double CashlessPayment
    {
        get; init;
    }

    [JsonPropertyName("withoutRevenue")]
    public double WithoutRevenue
    {
        get; init;
    }

    [JsonPropertyName("toDeposit")]
    public double ToDeposit
    {
        get; init;
    }

    [JsonPropertyName("toEntered")]
    public double ToEntered
    {
        get; init;
    }

    [JsonPropertyName("change")]
    public double Change
    {
        get; init;
    }

    [JsonPropertyName("withSalesReceipt")]
    public bool WithSalesReceipt
    {
        get; init;
    }
}

internal sealed record OrderEdgarModel
{
    [JsonPropertyName("order_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("problem")]
    public string Problem
    {
        get; init;
    }

    [JsonPropertyName("isProblematicDelivery")]
    public bool IsProblematicDelivery
    {
        get; init;
    }

    [JsonPropertyName("status")]
    public string Status
    {
        get; init;
    }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt
    {
        get; init;
    }

    [JsonPropertyName("deliveryTime")]
    public DateTime? DeliveryTime
    {
        get; init;
    }

    [JsonPropertyName("courierId")]
    public Guid? CourierId
    {
        get; init;
    }

    [JsonPropertyName("orderedProduct")]
    public IEnumerable<OrderedProductEdgarModel> Products
    {
        get; init;
    }

    [JsonPropertyName("comment")]
    public string? Comment
    {
        get; init;
    }

    [JsonPropertyName("totalSum")]
    public double TotalSum
    {
        get; init;
    }

    [JsonPropertyName("subTotalSum")]
    public double SubTotalSum
    {
        get; init;
    }

    [JsonPropertyName("discount")]
    public double Discount
    {
        get; init;
    }

    [JsonPropertyName("isDelivery")]
    public bool IsDelivery
    {
        get; init;
    }

    [JsonPropertyName("clientId")]
    public Guid? ClientId
    {
        get; init;
    }

    [JsonPropertyName("lastName")]
    public string LastName
    {
        get; init;
    }

    [JsonPropertyName("phone")]
    public string Phone
    {
        get; init;
    }

    [JsonPropertyName("card")]
    public string Card
    {
        get; init;
    }

    [JsonPropertyName("miscellaneous")]
    public string Miscellaneous
    {
        get; init;
    }

    [JsonPropertyName("house")]
    public string House
    {
        get; init;
    }

    [JsonPropertyName("building")]
    public string Building
    {
        get; init;
    }

    [JsonPropertyName("entrance")]
    public string Entrance
    {
        get; init;
    }

    [JsonPropertyName("floor")]
    public string Floor
    {
        get; init;
    }

    [JsonPropertyName("apartment")]
    public string Apartment
    {
        get; init;
    }

    [JsonPropertyName("intercom")]
    public string Intercom
    {
        get; init;
    }

    [JsonPropertyName("addressNote")]
    public string AddressNote
    {
        get; init;
    }

    [JsonPropertyName("isPickup")]
    public bool IsPickup
    {
        get; init;
    }

    [JsonPropertyName("streetId")]
    public Guid? StreetId
    {
        get; init;
    }

    [JsonPropertyName("districtId")]
    public Guid? DistrictId
    {
        get; init;
    }

    [JsonPropertyName("firstName")]
    public string FirstName
    {
        get; init;
    }

    [JsonPropertyName("middleName")]
    public string MiddleName
    {
        get; init;
    }

    [JsonPropertyName("isOutOfTurn")]
    public bool IsOutOfTurn
    {
        get; init;
    }

    [JsonPropertyName("payInfCash")]
    public double PayInfCash
    {
        get; init;
    }

    [JsonPropertyName("payInfBankCart")]
    public double PayInfBankCart
    {
        get; init;
    }

    [JsonPropertyName("payInfCashless")]
    public double PayInfCashless
    {
        get; init;
    }

    [JsonPropertyName("payInfWithoutRev")]
    public double PayInfWithoutRev
    {
        get; init;
    }

    [JsonPropertyName("payInfToDeposit")]
    public double PayInfToDeposit
    {
        get; init;
    }

    [JsonPropertyName("payinfToEntered")]
    public double PayinfToEntered
    {
        get; init;
    }

    [JsonPropertyName("payInfChange")]
    public double PayInfChange
    {
        get; init;
    }

    [JsonPropertyName("payInfWithSalesReceipt")]
    public bool PayInfWithSalesReceipt
    {
        get; init;
    }

    [JsonPropertyName("post_id")]
    public Guid PostId
    {
        get; init;
    }

    [JsonPropertyName("employeepost_id")]
    public Guid EmployeePostId
    {
        get; init;
    }
}

internal sealed record OrderedProductEdgarResponse
{
    [JsonPropertyName("orderedproduct_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("productId")]
    public Guid ProductId
    {
        get; init;
    }

    [JsonPropertyName("count")]
    public double Count
    {
        get; init;
    }

    [JsonPropertyName("price")]
    public double Price
    {
        get; init;
    }

    [JsonPropertyName("totalPrice")]
    public double TotalPrice
    {
        get; init;
    }

    [JsonPropertyName("subTotalPrice")]
    public double SubTotalPrice
    {
        get; init;
    }

    [JsonPropertyName("discount")]
    public double? Discount
    {
        get; init;
    }

    [JsonPropertyName("comment")]
    public string? Comment
    {
        get; init;
    }

    [JsonPropertyName("orderAdditive")]
    public IEnumerable<OrderedAdditiveEdgarResponse>? Additives
    {
        get; init;
    }
}

internal sealed record OrderedAdditiveEdgarResponse
{
    [JsonPropertyName("additive_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("count")]
    public double Count
    {
        get; init;
    }

    [JsonPropertyName("price")]
    public double Price
    {
        get; init;
    }

    [JsonPropertyName("totalPrice")]
    public double TotalPrice
    {
        get; init;
    }

    [JsonPropertyName("subTotalPrice")]
    public double SubTotalPrice
    {
        get; init;
    }

    [JsonPropertyName("discount")]
    public double? Discount
    {
        get; init;
    }

    [JsonPropertyName("measure")]
    public string Measure
    {
        get; init;
    }
}

internal sealed record OrderEdgarResponse
{
    [JsonPropertyName("order_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("title")]
    public string Title
    {
        get; init;
    }

    [JsonPropertyName("dishes")]
    public IEnumerable<OrderedProductEdgarResponse>? Products
    {
        get; init;
    }

    [JsonPropertyName("terminal_id")]
    public Guid TerminalId
    {
        get; init;
    }

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; init;
    }

    [JsonPropertyName("employee_id")]
    public Guid EmployeeId
    {
        get; init;
    }

    [JsonPropertyName("status")]
    public string Status
    {
        get; init;
    }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt
    {
        get; init;
    }

    [JsonPropertyName("deliveryTime")]
    public DateTime? DeliveryTime
    {
        get; init;
    }

    [JsonPropertyName("courierId")]
    public Guid? CourierId
    {
        get; init;
    }

    [JsonPropertyName("comment")]
    public string? Comment
    {
        get; init;
    }

    [JsonPropertyName("totalSum")]
    public double TotalSum
    {
        get; init;
    }

    [JsonPropertyName("subTotalSum")]
    public double SubTotalSum
    {
        get; init;
    }

    [JsonPropertyName("discount")]
    public double Discount
    {
        get; init;
    }

    [JsonPropertyName("isDelivery")]
    public bool IsDelivery
    {
        get; init;
    }

    [JsonPropertyName("clientId")]
    public Guid? ClientId
    {
        get; init;
    }

    [JsonPropertyName("lastName")]
    public string LastName
    {
        get; init;
    }

    [JsonPropertyName("phone")]
    public string Phone
    {
        get; init;
    }

    [JsonPropertyName("card")]
    public string Card
    {
        get; init;
    }

    [JsonPropertyName("miscellaneous")]
    public string Miscellaneous
    {
        get; init;
    }

    [JsonPropertyName("house")]
    public string House
    {
        get; init;
    }

    [JsonPropertyName("building")]
    public string Building
    {
        get; init;
    }

    [JsonPropertyName("entrance")]
    public string Entrance
    {
        get; init;
    }

    [JsonPropertyName("floor")]
    public string Floor
    {
        get; init;
    }

    [JsonPropertyName("apartment")]
    public string Apartment
    {
        get; init;
    }

    [JsonPropertyName("intercom")]
    public string Intercom
    {
        get; init;
    }

    [JsonPropertyName("addressNote")]
    public string AddressNote
    {
        get; init;
    }

    [JsonPropertyName("isPickup")]
    public bool IsPickup
    {
        get; init;
    }

    [JsonPropertyName("streetId")]
    public Guid? StreetId
    {
        get; init;
    }

    [JsonPropertyName("districtId")]
    public Guid? DistrictId
    {
        get; init;
    }

    [JsonPropertyName("firstName")]
    public string FirstName
    {
        get; init;
    }

    [JsonPropertyName("middleName")]
    public string MiddleName
    {
        get; init;
    }

    [JsonPropertyName("isOutOfTurn")]
    public bool IsOutOfTurn
    {
        get; init;
    }

    [JsonPropertyName("isProblematicDelivery")]
    public bool IsProblematicDelivery
    {
        get; init;
    }

    [JsonPropertyName("paymentinfoId")]
    public Guid? PaymentInfoId
    {
        get; init;
    }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt
    {
        get; init;
    }

    [JsonPropertyName("payInfCash")]
    public double PayInfCash
    {
        get; init;
    }

    [JsonPropertyName("payInfBankCart")]
    public double PayInfBankCart
    {
        get; init;
    }

    [JsonPropertyName("payInfCashless")]
    public double PayInfCashless
    {
        get; init;
    }

    [JsonPropertyName("payInfWithoutRev")]
    public double PayInfWithoutRev
    {
        get; init;
    }

    [JsonPropertyName("payInfToDeposit")]
    public double PayInfToDeposit
    {
        get; init;
    }

    [JsonPropertyName("payinfToEntered")]
    public double PayinfToEntered
    {
        get; init;
    }

    [JsonPropertyName("payInfChange")]
    public double PayInfChange
    {
        get; init;
    }

    [JsonPropertyName("payInfWithSalesReceipt")]
    public bool PayInfWithSalesReceipt
    {
        get; init;
    }

    [JsonPropertyName("problem")]
    public string Problem
    {
        get; init;
    }

    [JsonPropertyName("employeepost_id")]
    public Guid EmployeePostId
    {
        get; init;
    }

    [JsonPropertyName("post_id")]
    public Guid PostId
    {
        get; init;
    }
}

internal sealed record EdgarOrderInfoResponse
{
    [JsonPropertyName("order")]
    public OrderEdgarResponse Order
    {
        get; init;
    }

    [JsonPropertyName("products")]
    public IEnumerable<OrderedProductEdgarResponse> OrderedProducts
    {
        get; init;
    }
}