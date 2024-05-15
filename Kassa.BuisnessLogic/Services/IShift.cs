using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IShift
{
    public MemberDto Member
    {
        get; 
    }

    public IObservableOnlyBehaviourSubject<bool> IsStarted
    {
        get;
    }

    public Task Start();

    public Task Exit();
    public Task End(string pincode);
    public Task TakeBreak(string pincode);
    public Task EndBreak();

    public ValueTask<ShiftDto> CreateDto();
}
