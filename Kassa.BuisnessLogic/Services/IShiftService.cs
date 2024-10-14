using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IShiftService : IInitializableService
{
    public IApplicationModelManager<ShiftDto> RuntimeShifts
    {
        get;
    }

    public IObservableOnlyBehaviourSubject<IShift?> CurrentShift
    {
        get;
    }

    public IObservableOnlyBehaviourSubject<ITerminalShift?> CurrentCashierShift
    {
        get;
    }

    public ValueTask<ShiftDto?> GetShiftById(Guid id);
    public Task UpdateShift(ShiftDto shift);
    public Task AddShift(ShiftDto shift);
    public Task DeleteShift(Guid id);
    public Task<IEnumerable<ShiftDto>> GetShifts();
}
