using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class EdgarShift : IShift
{
    internal ShiftDto? _shift;
    private readonly DateTime? _start;
    private readonly BehaviorSubject<bool> _isStarted;
    private readonly ShiftService _shiftService;
    private readonly MemberDto _member;
    private readonly PostExistsResponse _postExistsResponse;

    public EdgarShift(ShiftService shiftService, MemberDto member, bool isStarted, PostExistsResponse postExistsResponse)
    {
        _shiftService = shiftService;
        _member = member;
        _postExistsResponse = postExistsResponse;
        _start = postExistsResponse.CreatedPost.OpenDate;

        _isStarted = new(isStarted);
        IsStarted = new ObservableOnlyBehaviourSubject<bool>(_isStarted);

    }

    public MemberDto Member => _member;

    public IObservableOnlyBehaviourSubject<bool> IsStarted
    {
        get;
    }

    public async Task Start()
    {
        var shiftDto = CreateDto();
        var employeeApi = Locator.Current.GetRequiredService<IEmployeePostsApi>();

        var openPostRequest = new EmployeeOpenPostRequest(DateTime.Now, shiftDto.Id, 0, shiftDto.CashierShiftId);

        var message = await employeeApi.OpenPost(openPostRequest);

        if (message.Message.Equals("success", StringComparison.InvariantCultureIgnoreCase))
        {
            _isStarted.OnNext(true);
        }
        else
        {
            throw new DeveloperException("Не удалось начать смену");
        }

        shiftDto.Start = openPostRequest.OpenDate;

        _shiftService.RuntimeShifts.AddOrUpdate(shiftDto);
        _shiftService._currentShift.OnNext(this);
    }

    public async Task Exit()
    {
        var shiftDto = CreateDto();

        _shiftService.RuntimeShifts.AddOrUpdate(shiftDto);
        _shiftService._currentShift.OnNext(null);
    }

    public async Task TakeBreak(string pincode)
    {
        var shiftDto = CreateDto();

        var employeePostApi = Locator.Current.GetRequiredService<IEmployeePostsApi>();

        var now = DateTime.Now;

        await employeePostApi.StartBreak(new(now, shiftDto.Id));

        shiftDto.BreakStart = now;

        _shiftService.RuntimeShifts.AddOrUpdate(shiftDto);
        _shiftService._currentShift.OnNext(null);
    }

    public async Task EndBreak()
    {
        var shiftDto = CreateDto();

        var employeePostApi = Locator.Current.GetRequiredService<IEmployeePostsApi>();

        var now = DateTime.Now;

        await employeePostApi.EndBreak(new(now, shiftDto.Id));

        shiftDto.BreakEnd = now;

        _shiftService.RuntimeShifts.AddOrUpdate(shiftDto);
        _shiftService._currentShift.OnNext(null);
    }

    public async Task End(string pincode)
    {
        //TODO: Implement pincode check
        await End();
    }

    internal async Task End()
    {
        var shiftDto = CreateDto();

        var employeePostApi = Locator.Current.GetRequiredService<IEmployeePostsApi>();

        var now = DateTime.Now;

        await employeePostApi.ClosePost(new(now, shiftDto.Id));

        shiftDto.End = now;

        _shiftService.RuntimeShifts.AddOrUpdate(shiftDto);
        _shiftService._currentShift.OnNext(null);
    }

    public ShiftDto CreateDto()
    {
        _shift ??= new ShiftDto()
        {
            Id = _postExistsResponse.CreatedPost.PostId,
            IsStarted = _postExistsResponse.CreatedPost.IsOpen,
            MemberId = _member.Id,
            Start = _start,
            Number = _postExistsResponse.CreatedPost.PostId.GuidToPrettyInt(),
            BreakStart = _postExistsResponse.CreatedPost.BreakStart,
            BreakEnd = _postExistsResponse.CreatedPost.BreakEnd,
            ManagerId = _postExistsResponse.CreatedPost.ManagerId,
            CashierShiftId = _postExistsResponse.CreatedPost.TerminalShiftId
        };

        return _shift;
    }
}