using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public class AdditiveService(IAdditiveRepository repository) : IAdditiveService
{
    public SourceCache<Additive, int> RuntimeAdditives
    {
        get;
    } = new(x => x.Id);

    public bool IsInitialized
    {
        get; set;
    }
    public bool IsDisposed
    {
        get; set;
    }

    public async Task DecreaseAddtiveCount(Additive additive)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(additive);

        if (additive.Count <= 0)
        {
            throw new ArgumentException("Additive count must be greater than zero.", nameof(additive));
        }

        await UpdateAdditive(additive);

        RuntimeAdditives.AddOrUpdate(additive with
        {
            Count = additive.Count - 1
        });
    }
    public void Dispose()
    {
        RuntimeAdditives.Dispose();
        IsDisposed = true;
    }
    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
    }

    public async ValueTask<Additive?> GetAdditiveById(int additiveId)
    {
        this.ThrowIfNotInitialized();

        var additive = await repository.Get(additiveId);

        if (additive is not null)
        {
            RuntimeAdditives.AddOrUpdate(additive);
        }

        return additive;
    }

    public async ValueTask<IEnumerable<Additive>> GetAdditivesByProductId(int id)
    {
        this.ThrowIfNotInitialized();

        var additives = await repository.GetAdditivesByProductId(id);

        RuntimeAdditives.AddOrUpdate(additives);

        return additives;
    }

    public async Task IncreaseAdditiveCount(Additive additive)
    {
        this.ThrowIfNotInitialized();

        var foundedAdditive = await repository.Get(additive.Id);

        if (foundedAdditive is null)
        {
            throw new InvalidOperationException($"Additive with id {additive.Id} not found");
        }

        additive = foundedAdditive with
        {
            Count = foundedAdditive.Count + 1
        };

        await UpdateAdditive(additive);
    }

    public ValueTask Initialize()
    {
        IsInitialized = true;

        return ValueTask.CompletedTask;
    }

    public async Task UpdateAdditive(Additive additive)
    {
        this.ThrowIfNotInitialized();

        await repository.Update(additive);

        RuntimeAdditives.AddOrUpdate(additive);
    }
}
