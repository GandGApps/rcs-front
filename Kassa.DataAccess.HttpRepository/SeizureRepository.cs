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
internal sealed class SeizureRepository : IRepository<SeizureReason>
{
    private FrozenDictionary<Guid, SeizureReason> _seizures = FrozenDictionary<Guid, SeizureReason>.Empty;

    public Task Add(SeizureReason item) => throw new NotImplementedException();
    public Task Delete(SeizureReason item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public async Task<SeizureReason?> Get(Guid id)
    {
        if (_seizures.Count == 0)
        {
            await GetAll();
        }

        if (_seizures.TryGetValue(id, out var seizure))
        {
            return seizure;
        }

        return null;
    }
    public async Task<IEnumerable<SeizureReason>> GetAll()
    {
        var fundApi = Locator.Current.GetRequiredService<IFundApi>();

        var reasons = await fundApi.GetSeizures();

        var seizures = reasons.Select(x => new SeizureReason()
        {
            Id = x.Id,
            Name = x.Name ?? string.Empty,
            IsRequiredComment = x.IsRequiredComment is true
        }).ToArray();

        _seizures = seizures.ToFrozenDictionary(x => x.Id);

        return seizures;
    }
    public Task Update(SeizureReason item) => throw new NotImplementedException();
}
