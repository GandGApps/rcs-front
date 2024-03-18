using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IDistrictService: IInitializableService
{
    public SourceCache<DistrictDto, Guid> RuntimeDistricts
    {
        get;
    }

    public ValueTask<DistrictDto?> GetDistrictById(Guid id);
    public ValueTask UpdateDistrict(DistrictDto district);
    public ValueTask AddDistrict(DistrictDto district);
    public ValueTask DeleteDistrict(Guid id);
    public ValueTask<IEnumerable<DistrictDto>> GetDistricts();
    public ValueTask<IEnumerable<StreetDto>> GetStreets(DistrictDto districtDto);
}
