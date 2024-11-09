using System;
using System.Collections.Frozen;
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
internal sealed class CategoryRepository : IRepository<Category>, IEnableLogger
{

    private readonly FrozenMemoryCache<Category> _cache = new();
    private readonly IDishGroupApi _api;

    public CategoryRepository(IDishGroupApi api)
    {
        _api = api;
    }

    public Task Add(Category item) => ThrowHelper.ThrowNotSupportedException<Task>();

    public Task Delete(Category item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task DeleteAll() => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task<Category?> Get(Guid id)
    {
        if (_cache.IsEmpty)
        {
            this.Log().Error("Categories is null");
            return Task.FromResult<Category?>(null);
        }

        return Task.FromResult(_cache.TryGetValue(id, out var category) ? category : null);
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        var response = await _api.GetDishGroups();

        var categories = response.Select(ApiMapper.MapRequestToCategory).ToList();

        _cache.Refresh(categories);

        return categories;
    }

    public Task Update(Category item) => ThrowHelper.ThrowNotSupportedException<Task>();
}
