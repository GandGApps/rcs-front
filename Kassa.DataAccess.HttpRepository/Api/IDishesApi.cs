using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;

internal interface IDishesApi : IApiOfMemberToken
{
    [Get("/dishes")]
    public Task<IEnumerable<DishRequest>> GetDishes();

    [Put("/dishes")]
    public Task PutDish(DishRequest dish);

    [Post("/dishes/create")]
    public Task<DishRequest> AddDish(DishRequest dishModel);

    [Delete("/dishes")]
    public Task DeleteDish([AliasAs("dish_id")]Guid dishId);
}

internal sealed record DishRequest
{
    [JsonPropertyName("dish_id")]
    public Guid DishId
    {
        get; set;
    }

    [JsonPropertyName("title")]
    public string Title
    {
        get; set;
    }

    [JsonPropertyName("nomenclatureType")]
    public string NomenclatureType
    {
        get; set;
    }

    [JsonPropertyName("hotKey")]
    public int HotKey
    {
        get; set;
    }

    [JsonPropertyName("measureUnit")]
    public Guid? MeasureUnit
    {
        get; set;
    }

    [JsonPropertyName("image")]
    public int Image
    {
        get; set;
    }

    [JsonPropertyName("theNutValue")]
    public int TheNutritionalValue
    {
        get; set;
    }

    [JsonPropertyName("techCard")]
    public string TechCard
    {
        get; set;
    }

    [JsonPropertyName("allergens")]
    public List<Guid> Allergens
    {
        get; set;
    }

    [JsonPropertyName("cookTech")]
    public string CookTech
    {

        get; set;
    }

    [JsonPropertyName("isModifiable")]
    public bool IsModifiable
    {
        get; set;
    }

    [JsonPropertyName("modificators")]
    public List<Guid> Modificators
    {
        get; set;
    }

    [JsonPropertyName("drinkFactor")]
    public bool DrinkFactor
    {
        get; set;
    }

    [JsonPropertyName("bulk")]
    public string Bulk
    {
        get; set;
    }

    [JsonPropertyName("isPortion")]
    public bool IsPortion
    {
        get; set;
    }

    [JsonPropertyName("portions")]
    public List<Guid> Portions
    {
        get; set;
    }

    [JsonPropertyName("parentGroupId")]
    public Guid? ParentGroupId
    {
        get; set;
    }

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; set;
    }

    [JsonPropertyName("establishment_id")]
    public Guid? EstablishmentId
    {
        get; set;
    }

    [JsonPropertyName("ingrArray")]
    public List<string> Ingredients
    {
        get; set;
    }

    [JsonPropertyName("full_price")]
    public double FullPrice
    {
        get; set;
    }
}