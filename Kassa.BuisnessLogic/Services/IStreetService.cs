using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IStreetService : IInitializableService
{
    public SourceCache<StreetDto, Guid> Streets
    {
        get;
    }

    public ValueTask<StreetDto?> GetStreetById(Guid id);

    public ValueTask<IEnumerable<StreetDto>> GetAllStreets();
}
