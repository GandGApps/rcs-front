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
using Kassa.Shared.ServiceLocator;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class SeizureRepository : IRepository<SeizureReason>
{
    private readonly FrozenMemoryCache<SeizureReason> _cache = new();
    private readonly IFundApi _api;

    public SeizureRepository(IFundApi api)
    {
        _api = api;
    }

    public Task Add(SeizureReason item) => throw new NotImplementedException();
    public Task Delete(SeizureReason item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public async Task<SeizureReason?> Get(Guid id)
    {
        if (_cache.IsEmpty)
        {
            await GetAll();
        }

        if (_cache.TryGetValue(id, out var seizure))
        {
            return seizure;
        }

        return null;
    }
    public async Task<IEnumerable<SeizureReason>> GetAll()
    {
        var reasons = await _api.GetSeizures();

        var seizures = reasons.Select(x => new SeizureReason()
        {
            Id = x.Id,
            Name = x.Name ?? string.Empty,
            IsRequiredComment = x.IsRequiredComment is true
        }).ToArray();

        _cache.Refresh(seizures);

        return seizures;
    }
    public Task Update(SeizureReason item) => throw new NotImplementedException();
}
