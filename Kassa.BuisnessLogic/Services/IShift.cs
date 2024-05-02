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

    public Task Exit();
    public Task TakeBreak();
    public Task EndBreak();

    public ValueTask<ShiftDto> CreateDto();
}
