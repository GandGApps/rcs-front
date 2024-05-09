using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Dto;
internal sealed class EdgarShift : IShift
{
    internal ShiftDto? _shift;
    private readonly DateTime _start = DateTime.Now;
    private readonly BehaviorSubject<bool> _isStarted;
    private readonly ShiftService _shiftService;
    private readonly MemberDto _member;

    public EdgarShift(ShiftService shiftService, MemberDto member, bool isStarted)
    {
        _shiftService = shiftService;
        _member = member;

        _isStarted= new(isStarted);
        IsStarted = new ObservableOnlyBehaviourSubject<bool>(_isStarted);
    }

    public MemberDto Member => _member;

    public IObservableOnlyBehaviourSubject<bool> IsStarted
    {
        get;
    }

    public async Task Start()
    {
        var employeeApi = Locator.Current.GetRequiredService<IEmployeeApi>();

        var openPostRequest = new OpenPostRequest(DateTime.Now, 0);

        await employeeApi.OpenPost(openPostRequest);
    }

    public async Task Exit()
    {
        var shiftDto = await CreateDto();

        shiftDto.End = DateTime.Now;

        if (shiftDto.Id == Guid.Empty)
        {
            await _shiftService.AddShift(shiftDto);
        }
        else
        {
            await _shiftService.UpdateShift(shiftDto);
        }

        _shiftService._currentShift.OnNext(null);
    }

    public async Task TakeBreak(string pincode)
    {
        var shiftDto = await CreateDto();

        shiftDto.BreakStart = DateTime.Now;

        if (shiftDto.Id == Guid.Empty)
        {
            await _shiftService.AddShift(shiftDto);
            _shiftService._currentShift.OnNext(null);
            return;
        }

        await _shiftService.UpdateShift(shiftDto);
    }

    public async Task EndBreak()
    {
        var shiftDto = await CreateDto();
        shiftDto.BreakEnd = DateTime.Now;

        await _shiftService.UpdateShift(shiftDto);
    }

    public ValueTask<ShiftDto> CreateDto()
    {
        _shift ??= new ShiftDto()
        {
            ManagerId = null,
            MemberId = _member.Id,
            Start = _start,
            Number = _shiftService.RuntimeShifts.Count + 1
        };

        return new(_shift);
    }
}