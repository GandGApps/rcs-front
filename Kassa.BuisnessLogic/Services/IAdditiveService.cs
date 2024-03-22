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
    public Task DecreaseAddtiveCount(AdditiveDto additive, double count = 1);
    public Task IncreaseAdditiveCount(AdditiveDto additive, double count = 1);

    public SourceCache<AdditiveDto, Guid> RuntimeAdditives
    {
        get;
    }

    public Task UpdateAdditive(AdditiveDto additive);

    public ValueTask<AdditiveDto?> GetAdditiveById(Guid additiveId);
    public ValueTask<IEnumerable<AdditiveDto>> GetAdditivesByProductId(Guid id);
}
