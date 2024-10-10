using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface ITechcardApi: IUseMemberToken
{
    [Get("/dishes/techcards")]
    public Task<IEnumerable<TechcardEdgarResponse>> GetAllTechcards();
}

internal sealed record IngridientEdgarResponse
{
    [JsonPropertyName("id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("code")]
    public required string Code
    {
        get; init;
    }

    [JsonPropertyName("left")]
    public double Left
    {
        get; init;
    }

    [JsonPropertyName("name")]
    public required string Name
    {
        get; init;
    }

    [JsonPropertyName("netto")]
    public double Netto
    {
        get; init;
    }

    [JsonPropertyName("price")]
    public double Price
    {
        get; init;
    }

    [JsonPropertyName("title")]
    public required string Title
    {
        get; init;
    }

    [JsonPropertyName("brutto")]
    public double Brutto
    {
        get; init;
    }

    [JsonPropertyName("article")]
    public required string Article
    {
        get; init;
    }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt
    {
        get; init;
    }

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; init;
    }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt
    {
        get; init;
    }

    [JsonPropertyName("unit_price")]
    public double UnitPrice
    {
        get; init;
    }

    [JsonPropertyName("measureUnit")]
    public required string MeasureUnit
    {
        get; init;
    }

    [JsonPropertyName("prime_price")]
    public double PrimePrice
    {
        get; init;
    }

    [JsonPropertyName("terminal_id")]
    public Guid? TerminalId
    {
        get; init;
    }

    [JsonPropertyName("loss_percent")]
    public double? LossPercent
    {
        get; init;
    }

    [JsonPropertyName("warehouse_id")]
    public Guid? WarehouseId
    {
        get; init;
    }

    [JsonPropertyName("ingridient_id")]
    public Guid IngridientId
    {
        get; init;
    }

    [JsonPropertyName("product_ready")]
    public double ProductReady
    {
        get; init;
    }

    [JsonPropertyName("alcoholPercent")]
    public double AlcoholPercent
    {
        get; init;
    }

    [JsonPropertyName("ingridients_id")]
    public Guid IngridientsId
    {
        get; init;
    }

    [JsonPropertyName("measureunit_id")]
    public Guid? MeasureUnitId
    {
        get; init;
    }

    [JsonPropertyName("packaging_unit")]
    public required string PackagingUnit
    {
        get; init;
    }
}

internal sealed record TechcardEdgarResponse
{
    [JsonPropertyName("techcard_id")]
    public Guid TechcardId
    {
        get; init;
    }

    [JsonPropertyName("ingridients")]
    public required IEnumerable<IngridientEdgarResponse> Ingridients
    {
        get; init;
    } 

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; init;
    }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt
    {
        get; init;
    }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt
    {
        get; init;
    }
}