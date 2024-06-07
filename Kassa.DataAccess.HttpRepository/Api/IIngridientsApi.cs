using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IIngridientsApi: IUseMemberToken
{
    [Get("/ingridients/ingridients")]
    public Task<IEnumerable<IngredientResponse>> GetIngridients();

    [Post("/ingridients/update")]
    public Task UpdateIngridients(IngredientResponse ingridient);
}

internal sealed record IngridientRequest(
    [property: JsonPropertyName("ingr_id")] Guid Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("count")] double Count);


internal sealed record IngredientResponse
{
    [JsonPropertyName("ingridients_id")]
    public Guid IngredientsId
    {
        get; init;
    }

    [JsonPropertyName("title")]
    public string Title
    {
        get; init;
    }

    /*[JsonPropertyName("warehouse")]
    public string Warehouse
    {
        get; init;
    }*/

    [JsonPropertyName("terminal_id")]
    public Guid? TerminalId
    {
        get; init;
    }

    [JsonPropertyName("office_id")]
    public Guid? OfficeId
    {
        get; init;
    }

    [JsonPropertyName("left")]
    public double Left
    {
        get; init;
    }

    [JsonPropertyName("price")]
    public double Price
    {
        get; init;
    }

    [JsonPropertyName("packaging_unit")]
    public string PackagingUnit
    {
        get; init;
    }

    [JsonPropertyName("alcoholPercent")]
    public double AlcoholPercent
    {
        get; init;
    }

    [JsonPropertyName("code")]
    public string Code
    {
        get; init;
    }

    [JsonPropertyName("article")]
    public string Article
    {
        get; init;
    }

    
}