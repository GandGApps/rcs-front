using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IAdditiveApi
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

    [JsonPropertyName("office_id")]
    public Guid? OfficeId
    {
        get; init;
    }
}