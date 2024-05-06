using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IOrdersApi: IApiOfMemberToken
{
    [Get("/orders")]
    public Task<IEnumerable<OrderDetails>> GetOrders();

    [Post("/orders/create")]
    public Task AddOrder(OrderDetails order);

    public Task<OrderDetails> GetOrder([AliasAs("order_id")] Guid id);
}

internal sealed record OrderedProduct
{
    [property: JsonPropertyName("count")]
    public int Count
    {
        get; set;
    }

    [property: JsonPropertyName("price")]
    public double Price
    {
        get; set;
    }

    [property: JsonPropertyName("productId")]
    public string ProductId
    {
        get; set;
    }

    [property: JsonPropertyName("totalPrice")]
    public double TotalPrice
    {
        get; set;
    }

    [property: JsonPropertyName("subTotalPrice")]
    public double SubTotalPrice
    {
        get; set;
    }

    [property: JsonPropertyName("comment")]
    public string? Comment
    {
        get; set;
    }

    [property: JsonPropertyName("additives")]
    public List<OrderedAdditive> Additives
    {
        get; set;
    } = [];
}

internal sealed record OrderedAdditive
{
    [property: JsonPropertyName("count")]
    public string Count
    {
        get; set;
    }

    [property: JsonPropertyName("price")]
    public double Price
    {
        get; set;
    }

    [property: JsonPropertyName("AdditiveId")]
    public string AdditiveId
    {
        get; set;
    }

    [property: JsonPropertyName("totalPrice")]
    public double TotalPrice
    {
        get; set;
    }

    [property: JsonPropertyName("subTotalPrice")]
    public double SubTotalPrice
    {
        get; set;
    }
}

internal sealed record OrderDetails
{
    [property: JsonPropertyName("orderId")]
    public Guid? Id
    {
        get; set;
    }

    [property: JsonPropertyName("orderedProduct")]
    public IEnumerable<OrderedProduct> OrderedProduct
    {
        get; set;
    }

    [property: JsonPropertyName("status")]
    public string Status
    {
        get; set;
    }

    [property: JsonPropertyName("deliveryTime")]
    public string DeliveryTime
    {
        get; set;
    }

    [property: JsonPropertyName("courierId")]
    public Guid? CourierId
    {
        get; set;
    }

    [property: JsonPropertyName("comment")]
    public string? Comment
    {
        get; set;
    }

    [property: JsonPropertyName("totalSum")]
    public string TotalSum
    {
        get; set;
    }

    [property: JsonPropertyName("discount")]
    public string Discount
    {
        get; set;
    }

    [property: JsonPropertyName("isDelivery")]
    public bool IsDelivery
    {
        get; set;
    }

    [property: JsonPropertyName("clientId")]
    public Guid? ClientId
    {
        get; set;
    }

    [property: JsonPropertyName("lastName")]
    public string LastName
    {
        get; set;
    }

    [property: JsonPropertyName("phone")]
    public string Phone
    {
        get; set;
    }

    [property: JsonPropertyName("card")]
    public string Card
    {
        get; set;
    }

    [property: JsonPropertyName("miscellaneous")]
    public string Miscellaneous
    {
        get; set;
    }

    [property: JsonPropertyName("house")]
    public string House
    {
        get; set;
    }

    [property: JsonPropertyName("building")]
    public string Building
    {
        get; set;
    }

    [property: JsonPropertyName("entrance")]
    public string Entrance
    {
        get; set;
    }

    [property: JsonPropertyName("floor")]
    public string Floor
    {
        get; set;
    }

    [property: JsonPropertyName("apartment")]
    public string Apartment
    {
        get; set;
    }

    [property: JsonPropertyName("intercom")]
    public string Intercom
    {
        get; set;
    }

    [property: JsonPropertyName("addressNote")]
    public string AddressNote
    {
        get; set;
    }

    [property: JsonPropertyName("isPickup")]
    public bool IsPickup
    {
        get; set;
    }

    [property: JsonPropertyName("streetId")]
    public Guid? StreetId
    {
        get; set;
    }

    [property: JsonPropertyName("districtId")]
    public Guid? DistrictId
    {
        get; set;
    }

    [property: JsonPropertyName("firstName")]
    public string FirstName
    {
        get; set;
    }

    [property: JsonPropertyName("middleName")]
    public string MiddleName
    {
        get; set;
    }

    [property: JsonPropertyName("isOutOfTurn")]
    public bool IsOutOfTurn
    {
        get; set;
    }

    [property: JsonPropertyName("isProblematicDelivery")]
    public bool IsProblematicDelivery
    {
        get; set;
    }

    [property: JsonPropertyName("isModified")]
    public string IsModified
    {
        get; set;
    }

    [property: JsonPropertyName("payInfCash")]
    public double PayInfCash
    {
        get; set;
    }

    [property: JsonPropertyName("payInfBankCart")]
    public double PayInfBankCart
    {
        get; set;
    }

    [property: JsonPropertyName("payInfCashless")]
    public double PayInfCashless
    {
        get; set;
    }

    [property: JsonPropertyName("payInfWithoutRev")]
    public double PayInfWithoutRev
    {
        get; set;
    }

    [property: JsonPropertyName("payInfToDeposit")]
    public double PayInfToDeposit
    {
        get; set;
    }

    [property: JsonPropertyName("payinfToEntered")]
    public double PayinfToEntered
    {
        get; set;
    }

    [property: JsonPropertyName("payInfChange")]
    public double PayInfChange
    {
        get; set;
    }

    [property: JsonPropertyName("payInfWithSalesReceipt")]
    public bool PayInfWithSalesReceipt
    {
        get; set;
    }
}
