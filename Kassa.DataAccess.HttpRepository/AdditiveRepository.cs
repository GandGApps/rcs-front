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
internal sealed class AdditiveRepository : IAdditiveRepository, IEnableLogger
{
    private readonly IAdditiveApi _additiveApi;

    private FrozenDictionary<Guid, Additive>? _additives;

    public AdditiveRepository(IAdditiveApi additiveApi)
    {
        _additiveApi = additiveApi;
    }

    public Task Add(Additive item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task Delete(Additive item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task DeleteAll() => ThrowHelper.ThrowNotSupportedException<Task>();

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

        var additivesResponse = await _additiveApi.GetAdditives();

        var additives = additivesResponse.Select(ApiMapper.MapAdditiveEdgarToAdditive).ToList();

        _additives = additives.ToFrozenDictionary(additive => additive.Id);

        return additives;
    }

    public Task Update(Additive item)
    {
        // TODO: Implement update
        this.Log().Error("Update is not supported for additives");

        return Task.CompletedTask;
    }
}
