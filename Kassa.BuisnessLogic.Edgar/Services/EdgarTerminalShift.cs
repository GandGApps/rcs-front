using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.Shared;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal class EdgarTerminalShift : ITerminalShift
{
    private CashierShiftDto? _cashierShift;

    private readonly MemberDto _manager;
    private readonly TerminalPostExistsResponse _postExistsResponse;
    private readonly BehaviorSubject<bool> _isStarted;
    private readonly DateTime? _start;
    private readonly ShiftService _shiftService;

    public EdgarTerminalShift(MemberDto manager, TerminalPostExistsResponse postExistsResponse, ShiftService shiftService)
    {
        _manager = manager;

        _postExistsResponse = postExistsResponse;

        _start = postExistsResponse.Posts.OpenDate;
        _shiftService = shiftService;

        _isStarted = new(postExistsResponse.Posts.IsOpen);

        IsStarted = new(_isStarted);
    }

    public MemberDto Manager => _manager;

    public ObservableOnlyBehaviourSubject<bool> IsStarted
    {
        get;
    }

    public async Task Start()
    {
        var shift = await CreateDto();

        var terminalPostApi = Locator.Current.GetRequiredService<ITerminalPostApi>();
        var openShiftRequest = new TerminalOpenPostRequest(DateTime.Now, shift.Id, 0);

        await terminalPostApi.OpenPost(openShiftRequest);

        _isStarted.OnNext(true);
    }

    public async Task End()
    {
        var shift = await CreateDto();

        var terminalPostApi = Locator.Current.GetRequiredService<ITerminalPostApi>();
        var closeShiftRequest = new TerminalClosePostRequest(DateTime.Now, shift.Id);

        await terminalPostApi.ClosePost(closeShiftRequest);

        if (_shiftService.CurrentShift.Value is EdgarShift edgarShift)
        {
            await edgarShift.End();
        }

        _shiftService._currentCashierShift.OnNext(null);
        _shiftService._currentShift.OnNext(null);
    }

    public ValueTask<CashierShiftDto> CreateDto()
    {
        _cashierShift ??= new CashierShiftDto()
        {
            Id = _postExistsResponse.PostId,
            IsStarted = _postExistsResponse.Posts.IsOpen,
            MemberId = _manager.Id,
            Start = _start,
            Number = _postExistsResponse.Posts.PostId.GuidToPrettyInt(),
        };

        return new(_cashierShift);
    }
}
