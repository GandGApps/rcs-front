using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public class AdditiveService : IAdditiveService
{
    public SourceCache<Additive, int> RuntimeAdditives
    {
        get;
    }
    public bool IsInitialized
    {
        get;
    }
    public bool IsDisposed
    {
        get;
    }

    public async Task DecreaseAddtiveCount(Additive additive)
    {
        if (additive is null)
        {

            throw new ArgumentNullException(nameof(additive));
        }

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
    }
    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
    }

    public ValueTask<Additive?> GetAdditiveById(int additiveId) => throw new NotImplementedException();
    public Task IncreaseAdditiveCount(Additive additive) => throw new NotImplementedException();
    public ValueTask Initialize() => throw new NotImplementedException();
    public Task UpdateAdditive(Additive additive) => throw new NotImplementedException();
}
