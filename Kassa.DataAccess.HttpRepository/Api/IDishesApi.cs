using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;

internal interface IDishesApi : IUseMemberToken
{
    [Get("/dishes")]
    public Task<IEnumerable<DishRequest>> GetDishes();

    [Put("/dishes")]
    public Task PutDish(DishRequest dish);

    [Post("/dishes/create")]
    public Task<DishRequest> AddDish(DishRequest dishModel);

    [Delete("/dishes")]
    public Task DeleteDish([AliasAs("dish_id")] Guid dishId);
}

internal sealed record DishRequest
{
    [JsonPropertyName("dish_id")]
    public Guid DishId
    {
        get; init;
    }

    [JsonPropertyName("title")]
    public required string Title
    {
        get; init;
    }

    [JsonPropertyName("nomenclatureType")]
    public string? NomenclatureType
    {
        get; init;
    }

    [JsonPropertyName("hotKey")]
    public int HotKey
    {
        get; init;
    }

    [JsonPropertyName("measureUnit")]
    public string? MeasureUnit
    {
        get; init;
    }

    [JsonPropertyName("image")]
    public int? Image
    {
        get; init;
    }

    [JsonPropertyName("theNutValue")]
    public int TheNutValue
    {
        get; init;
    }

    [JsonPropertyName("techCard")]
    public string? TechCard
    {
        get; init;
    }
    /*
        [JsonPropertyName("allergens")]
        public List<string> Allergens { get; init; }*/

    [JsonPropertyName("cookTech")]
    public string? CookTech
    {
        get; init;
    }

    [JsonPropertyName("isModifiable")]
    public bool IsModifiable
    {
        get; init;
    }

    /*[JsonPropertyName("modificators")]
    public List<string> Modificators { get; init; }*/

    [JsonPropertyName("drinkFactor")]
    public bool DrinkFactor
    {
        get; init;
    }

    [JsonPropertyName("bulk")]
    public string? Bulk
    {
        get; init;
    }

    [JsonPropertyName("isPortion")]
    public bool IsPortion
    {
        get; init;
    }

    [JsonPropertyName("portions")]
    public List<string>? Portions
    {
        get; init;
    }

    [JsonPropertyName("parentGroupId")]
    public Guid? ParentGroupId
    {
        get; init;
    }

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; init;
    }

    [JsonPropertyName("establishment_id")]
    public Guid? EstablishmentId
    {
        get; init;
    }

    /*[JsonPropertyName("ingrArray")]
    public List<string> IngrArray { get; init; }*/

    [JsonPropertyName("full_price")]
    public double FullPrice
    {
        get; init;
    }

    [JsonPropertyName("techcard_id")]
    public Guid? TechCardId
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

    [JsonPropertyName("category_color")]
    public string? Color
    {
        get; init;
    }

}

[JsonSerializable(typeof(DishRequest))]
internal partial class DishRequestJsonContext : JsonSerializerContext
{

}