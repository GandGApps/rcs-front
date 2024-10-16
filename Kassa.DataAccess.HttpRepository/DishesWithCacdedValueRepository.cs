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
    private readonly FrozenMemoryCache<Product> _cache = new();
    private readonly IDishesApi _api;

    public DishesWithCacdedValueRepository(IDishesApi api)
    {
        _api = api;
    }

    public async Task Add(Product item)
    {
        var dishRequest = ApiMapper.MapDishToRequest(item);

        await _api.AddDish(dishRequest);
    }

    public Task Delete(Product item)
    {
        return _api.DeleteDish(item.Id);
    }

    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Product?> Get(Guid id)
    {
        Product? product = null;

        if (_cache is not null)
        {
            _cache.TryGetValue(id, out product);
        }

        return Task.FromResult(product);
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var response = await _api.GetDishes();

        var dishes = response.Select(ApiMapper.MapRequestToDish).ToList();

        _cache.Refresh(dishes);

        return dishes;
    }

    public Task Update(Product item)
    {
        var dishRequest = ApiMapper.MapDishToRequest(item);

        return _api.PutDish(dishRequest);
    }
}
