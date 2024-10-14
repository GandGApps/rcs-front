using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
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
internal sealed class ReceiptRepository : IRepository<Receipt>, IEnableLogger
{
    private readonly FrozenMemoryCache<Receipt> _cache = new();
    private readonly ITechcardApi _api;

    public ReceiptRepository(ITechcardApi api)
    {
        _api = api;
    }

    public Task Add(Receipt item) => throw new NotImplementedException();
    public Task Delete(Receipt item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Receipt?> Get(Guid id)
    {
        if (_cache.IsEmpty)
        {
            return Task.FromResult<Receipt?>(null);
        }

        if (_cache.TryGetValue(id, out var receipt))
        {

            return Task.FromResult<Receipt?>(receipt);
        }

        return Task.FromResult<Receipt?>(null);
    }
    public async Task<IEnumerable<Receipt>> GetAll()
    {
        var techcards = await _api.GetAllTechcards();

        var receipts = techcards.Select(ApiMapper.MapTechcardToReceipt).ToList();

        _cache.Refresh(receipts);

        return receipts;
    }

    public Task Update(Receipt item)
    {
        this.Log().Error("Update method is not implemented");

        return Task.CompletedTask;
    }
}
