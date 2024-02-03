using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Services;
public interface IAdditiveService : IInitializableService
{
    public Task DecreaseAddtiveCount(AdditiveDto additive);
    public Task IncreaseAdditiveCount(AdditiveDto additive);

    public SourceCache<AdditiveDto, int> RuntimeAdditives
    {
        get;
    }

    public Task UpdateAdditive(AdditiveDto additive);

    public ValueTask<AdditiveDto?> GetAdditiveById(int additiveId);
    public ValueTask<IEnumerable<AdditiveDto>> GetAdditivesByProductId(int id);
}
