using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface ICashierShift
{
    public MemberDto Manager
    {
        get;
    }

    public ObservableOnlyBehaviourSubject<bool> IsOpen
    {
        get;
    }

    public Task Start();
    public Task End();

    public ValueTask<CashierShiftDto> GetCashierShiftAsync();
}
