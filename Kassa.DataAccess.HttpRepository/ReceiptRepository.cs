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
    private FrozenDictionary<Guid, Receipt>? _receipts;

    public Task Add(Receipt item) => throw new NotImplementedException();
    public Task Delete(Receipt item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Receipt?> Get(Guid id)
    {
        if (_receipts is null)
        {
            return Task.FromResult<Receipt?>(null);
        }

        if (_receipts.TryGetValue(id, out var receipt))
        {

            return Task.FromResult<Receipt?>(receipt);
        }

        return Task.FromResult<Receipt?>(null);
    }
    public async Task<IEnumerable<Receipt>> GetAll()
    {
        var techcardApi = RcsLocator.GetRequiredService<ITechcardApi>();

        var techcards = await techcardApi.GetAllTechcards();

        var receipts = techcards.Select(ApiMapper.MapTechcardToReceipt).ToList();

        _receipts = receipts.ToDictionary(x => x.Id).ToFrozenDictionary();

        return receipts;
    }

    public Task Update(Receipt item)
    {
        this.Log().Error("Update method is not implemented");

        return Task.CompletedTask;
    }
}
