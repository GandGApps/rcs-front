using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public interface IAdditiveService: IInitializableService
{
    public Task DecreaseAddtiveCount(Additive additive);
    public Task IncreaseAdditiveCount(Additive additive);

    public SourceCache<Additive, int> RuntimeAdditives
    {
        get;
    }

    public Task UpdateAdditive(Additive additive);

    public ValueTask<Additive?> GetAdditiveById(int additiveId);
}
