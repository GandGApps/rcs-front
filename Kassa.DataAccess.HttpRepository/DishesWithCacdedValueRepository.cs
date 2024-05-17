using System;
using System.Collections.Frozen;
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
internal sealed class DishesWithCacdedValueRepository : IRepository<Product>
{
    private FrozenDictionary<Guid, Product>? _products;
    public async Task Add(Product item)
    {
        var dishesApi = Locator.Current.GetRequiredService<IDishesApi>();

        var dishRequest = ApiMapper.MapDishToRequest(item);

        await dishesApi.AddDish(dishRequest);
    }

    public Task Delete(Product item)
    {
        var dishesApi = Locator.Current.GetRequiredService<IDishesApi>();

        return dishesApi.DeleteDish(item.Id);
    }

    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Product?> Get(Guid id)
    {
        Product? product = null;

        if (_products is not null)
        {
            _products.TryGetValue(id, out product);
        }

        return Task.FromResult(product);
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var dishesApi = Locator.Current.GetRequiredService<IDishesApi>();

        var response = await dishesApi.GetDishes();

        var dishes = response.Select(ApiMapper.MapRequestToDish).ToList();

        _products = dishes.ToFrozenDictionary(d => d.Id);

        return dishes;
    }

    public Task Update(Product item)
    {
        var dishesApi = Locator.Current.GetRequiredService<IDishesApi>();

        var dishRequest = ApiMapper.MapDishToRequest(item);

        return dishesApi.PutDish(dishRequest);
    }
}
