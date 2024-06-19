using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IAdditiveApi : IUseMemberToken
{
    [Get("/dishes/additives")]
    public Task<IEnumerable<AdditiveEdgarModel>> GetAdditives();
}

internal sealed record AdditiveEdgarModel
{
    [JsonPropertyName("add_id")]
    public Guid Id
    {
        get; init;
    }

    [JsonPropertyName("name")]
    public string Name
    {
        get; init;
    }

    [JsonPropertyName("nomenclatureType")]
    public string NomenclatureType
    {
        get; init;
    }

    [JsonPropertyName("accountingCategory")]
    public string AccountingCategory
    {
        get; init;
    }

    [JsonPropertyName("parentgroup_id")]
    public Guid? ParentGroupId
    {
        get; init;
    }

    [JsonPropertyName("article")]
    public int Article
    {
        get; init;
    }

    [JsonPropertyName("code")]
    public int Code
    {
        get; init;
    }

    [JsonPropertyName("warehouse")]
    public Guid? Warehouse
    {
        get; init;
    }

    [JsonPropertyName("modificator_value")]
    public double ModificatorValue
    {
        get; init;
    }

    [JsonPropertyName("dishes")]
    public AdditiveToDishFk[]? Dishes
    {
        get; init;
    } = [];

    [JsonPropertyName("techcard_id")]
    public Guid? TechcardId
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

internal sealed record AdditiveToDishFk
{
    [JsonPropertyName("dish_id")]
    public Guid DishId
    {
        get; init;
    }
}