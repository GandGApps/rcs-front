using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Services;
public interface ICourierService: IInitializableService
{
    public IApplicationModelManager<CourierDto> RuntimeCouriers
    {
        get;
    }
    public ValueTask<CourierDto?> GetCouriersById(Guid id);
    public Task UpdateCourier(CourierDto courier);
    public Task AddClient(CourierDto courier);
    public Task DeleteCourier(Guid id);
    public Task<IEnumerable<CourierDto>> GetCouriers();
}
