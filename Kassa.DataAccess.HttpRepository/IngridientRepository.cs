using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal class IngridientRepository : IRepository<Ingredient>
{
    public Task Add(Ingredient item) => throw new NotImplementedException();
    public Task Delete(Ingredient item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Ingredient?> Get(Guid id) => throw new NotImplementedException();
    public async Task<IEnumerable<Ingredient>> GetAll()
    {
        var ingridientsApi = Locator.Current.GetRequiredService<IIngridientsApi>();

        var ingridinets = await ingridientsApi.GetIngridients();

        return ingridinets.Select(ApiMapper.MapRequestToIngredient).ToList();
    }

    public Task Update(Ingredient item)
    {
        var ingridientsApi = Locator.Current.GetRequiredService<IIngridientsApi>();

        var ingridientRequest = ApiMapper.MapIngredientToRequest(item);

        return ingridientsApi.UpdateIngridients(ingridientRequest);
    }
}
