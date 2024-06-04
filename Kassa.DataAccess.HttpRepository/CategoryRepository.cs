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
internal sealed class CategoryRepository : IRepository<Category>, IEnableLogger
{

    private FrozenDictionary<Guid, Category>? _categories;

    public Task Add(Category item) => throw new NotImplementedException();

    public Task Delete(Category item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Category?> Get(Guid id)
    {
        if (_categories is null)
        {
            this.Log().Error("Categories is null");
            return Task.FromResult<Category?>(null);
        }

        return Task.FromResult(_categories.TryGetValue(id, out var category) ? category : null);
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        var categoryApi = Locator.Current.GetRequiredService<IDishGroupApi>();

        var response = await categoryApi.GetDishGroups();

        var categories = response.Select(ApiMapper.MapRequestToCategory).ToList();

        _categories = categories.ToFrozenDictionary(x => x.Id);

        return categories;
    }

    public Task Update(Category item) => throw new NotImplementedException();
}
