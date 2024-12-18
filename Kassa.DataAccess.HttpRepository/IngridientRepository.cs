﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class IngridientRepository : IRepository<Ingredient>, IEnableLogger
{
    private readonly FrozenMemoryCache<Ingredient> _cache = new();
    private readonly IIngridientsApi _api;

    public IngridientRepository(IIngridientsApi api)
    {
        _api = api;
    }

    public Task Add(Ingredient item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task Delete(Ingredient item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task DeleteAll() => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task<Ingredient?> Get(Guid id) => ThrowHelper.ThrowNotSupportedException<Task<Ingredient?>>();
    public async Task<IEnumerable<Ingredient>> GetAll()
    {
        if (!_cache.IsExpired)
        {
            return _cache.Values;
        }

        var ingridientsApi = _api;

        var ingridinets = await ingridientsApi.GetIngridients();

        var result = ingridinets.Select(ApiMapper.MapEdgarModelToIngredient).ToList(); 

        _cache.Refresh(result);

        return result;
    }

    public Task Update(Ingredient item)
    {
        this.Log().Error("Update method is not implemented");

        return ThrowHelper.ThrowNotSupportedException<Task>();


        /*var ingridientsApi = RcsKassa.GetRequiredService<IIngridientsApi>();

        var ingridientRequest = ApiMapper.MapIngredientToEdgarModel(item);

        return ingridientsApi.UpdateIngridients(ingridientRequest);*/
    }
}
