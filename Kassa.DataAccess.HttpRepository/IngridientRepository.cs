using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class IngridientRepository : IRepository<Ingredient>, IEnableLogger
{
    private readonly FrozenMemoryCache<Ingredient> _cache = new(TimeSpan.FromMinutes(15));

    public Task Add(Ingredient item) => throw new NotImplementedException();
    public Task Delete(Ingredient item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Ingredient?> Get(Guid id) => throw new NotImplementedException();
    public async Task<IEnumerable<Ingredient>> GetAll()
    {
        if (!_cache.IsExpired)
        {
            return _cache.Values;
        }

        var ingridientsApi = RcsLocator.GetRequiredService<IIngridientsApi>();

        var ingridinets = await ingridientsApi.GetIngridients();

        var result = ingridinets.Select(ApiMapper.MapEdgarModelToIngredient).ToList(); 

        _cache.Refresh(result);

        return result;
    }

    public Task Update(Ingredient item)
    {
        this.Log().Error("Update method is not implemented");

        throw new DeveloperException("Логическая ошибка, обратитесь к Баястану");


        var ingridientsApi = RcsLocator.GetRequiredService<IIngridientsApi>();

        var ingridientRequest = ApiMapper.MapIngredientToEdgarModel(item);

        return ingridientsApi.UpdateIngridients(ingridientRequest);
    }
}
