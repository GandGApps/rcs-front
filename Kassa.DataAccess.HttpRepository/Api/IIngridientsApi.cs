using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IIngridientsApi: IApiOfMemberToken
{
    [Get("/ingridients/ingridients")]
    public Task<IEnumerable<IngridientRequest>> GetIngridients();

    [Post("/ingridients/update")]
    public Task UpdateIngridients(IngridientRequest ingridient);
}

internal sealed record IngridientRequest(
    [property: JsonPropertyName("ingr_id")] Guid Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("count")] double Count);
