using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;

namespace Kassa.BuisnessLogic.Edgar.Dto;
internal sealed class EdgarShift(ShiftService shiftService, MemberDto member): IShift
{
    internal ShiftDto? _shift;
    private readonly DateTime _start = DateTime.Now;

    public MemberDto Member => member;

    public async Task Exit()
    {
        var shiftDto = await CreateDto();

        shiftDto.End = DateTime.Now;

        if (shiftDto.Id == Guid.Empty)
        {
            await shiftService.AddShift(shiftDto);
        }
        else
        {
            await shiftService.UpdateShift(shiftDto);
        }

        shiftService._currentShift.OnNext(null);
    }

    public async Task TakeBreak()
    {
        var shiftDto = await CreateDto();

        shiftDto.BreakStart = DateTime.Now;

        if (shiftDto.Id == Guid.Empty)
        {
            await shiftService.AddShift(shiftDto);
            shiftService._currentShift.OnNext(null);
            return;
        }

        await shiftService.UpdateShift(shiftDto);
    }

    public async Task EndBreak()
    {
        var shiftDto = await CreateDto();
        shiftDto.BreakEnd = DateTime.Now;

        await shiftService.UpdateShift(shiftDto);
    }

    public ValueTask<ShiftDto> CreateDto()
    {
        _shift ??= new ShiftDto()
        {
            ManagerId = null,
            MemberId = member.Id,
            Start = _start,
            Number = shiftService.RuntimeShifts.Count + 1
        };

        return new(_shift);
    }
}