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
internal sealed class AdditiveRepository : IAdditiveRepository, IEnableLogger
{
    private FrozenDictionary<Guid, Additive>? _additives;
    public Task Add(Additive item) => throw new NotImplementedException();
    public Task Delete(Additive item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();

    public Task<Additive?> Get(Guid id)
    {
        if (_additives is null)
        {
            this.Log().Warn("Additives are not loaded yet");
            return Task.FromResult<Additive?>(null);
        }

        var additive = _additives.TryGetValue(id, out var value) ? value : null;

        return Task.FromResult(additive);
    }

    public Task<IEnumerable<Additive>> GetAdditivesByProductId(Guid productId)
    {
        if (_additives is null)
        {
            this.Log().Warn("Additives are not loaded yet");
            return Task.FromResult(Enumerable.Empty<Additive>());
        }

        var additives = _additives.Values.Where(additive => additive.ProductIds.Contains(productId));

        return Task.FromResult(additives);
    }

    public async Task<IEnumerable<Additive>> GetAll()
    {
        var additiveApi = Locator.Current.GetRequiredService<IAdditiveApi>();

        var additivesResponse = await additiveApi.GetAdditives();

        var additives = additivesResponse.Select(ApiMapper.MapAdditiveEdgarToAdditive).ToList();

        _additives = additives.ToFrozenDictionary(additive => additive.Id);

        return additives;
    }

    public Task Update(Additive item) => throw new NotImplementedException();
}
