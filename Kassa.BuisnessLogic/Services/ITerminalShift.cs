using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface ITerminalShift
{
    public MemberDto Manager
    {
        get;
    }

    public ObservableOnlyBehaviourSubject<bool> IsStarted
    {
        get;
    }

    public Task Start();
    public Task End();



    public CashierShiftDto CreateDto();
}
