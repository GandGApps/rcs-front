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
internal class EdgarManagerShift : ITerminalShift
{
    private CashierShiftDto? _cashierShift;

    private readonly MemberDto _manager;
    private readonly PostExistsResponse _postExistsResponse;
    private readonly BehaviorSubject<bool> _isStarted = new(false);
    private readonly DateTime? _start;
    private readonly ShiftService _shiftService;

    public EdgarManagerShift(MemberDto manager, PostExistsResponse postExistsResponse, ShiftService shiftService)
    {
        _manager = manager;
        IsStarted = new(_isStarted);
        _postExistsResponse = postExistsResponse;

        _start = postExistsResponse.CreatedPost.OpenDate;
        _shiftService = shiftService;
    }

    public MemberDto Manager => _manager;

    public ObservableOnlyBehaviourSubject<bool> IsStarted
    {
        get;
    }

    public async Task Start()
    {
        var shift = await GetCashierShiftAsync();

        var terminalPostApi = Locator.Current.GetRequiredService<ITerminalPostApi>();
        var openShiftRequest = new TerminalOpenPostRequest(DateTime.Now, shift.Id, 0);

        await terminalPostApi.OpenPost(openShiftRequest);

        _isStarted.OnNext(true);
    }

    public async Task End()
    {
        var shift = await GetCashierShiftAsync();

        var terminalPostApi = Locator.Current.GetRequiredService<ITerminalPostApi>();
        var closeShiftRequest = new TerminalClosePostRequest(DateTime.Now, shift.Id);

        await terminalPostApi.ClosePost(closeShiftRequest);

        _shiftService._currentCashierShift.OnNext(null);
    }

    public ValueTask<CashierShiftDto> GetCashierShiftAsync()
    {
        _cashierShift ??= new CashierShiftDto()
        {
            Id = _postExistsResponse.CreatedPost.PostId,
            IsStarted = _postExistsResponse.CreatedPost.IsOpen,
            MemberId = _manager.Id,
            Start = _start,
            Number = _postExistsResponse.CreatedPost.PostId.GuidToPrettyInt(),
            BreakStart = _postExistsResponse.CreatedPost.BreakStart,
            BreakEnd = _postExistsResponse.CreatedPost.BreakEnd
        };

        return new(_cashierShift);
    }
}
