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
internal sealed class CategoryRepository : IRepository<Category>
{
    public Task Add(Category item) => throw new NotImplementedException();

    public Task Delete(Category item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Category?> Get(Guid id) => throw new NotImplementedException();
    
    public async Task<IEnumerable<Category>> GetAll()
    {
        var categoryApi = Locator.Current.GetRequiredService<IDishGroupApi>();

        var response = await categoryApi.GetDishGroups();

        return response.Select(ApiMapper.MapRequestToCategory).ToList();
    }

    public Task Update(Category item) => throw new NotImplementedException();
}
