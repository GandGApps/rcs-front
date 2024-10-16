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
internal sealed class EdgarTerminalShift : ITerminalShift
{
    private CashierShiftDto? _cashierShift;

    private readonly MemberDto _manager;
    private readonly TerminalPostExistsResponse _postExistsResponse;
    private readonly BehaviorSubject<bool> _isStarted;
    private readonly DateTime? _start;
    private readonly ShiftService _shiftService;
    private readonly ITerminalPostApi _terminalPostApi;
    private readonly IReportShiftService _reportShiftService;

    public EdgarTerminalShift(ITerminalPostApi terminalPostApi, IReportShiftService reportShiftService, MemberDto manager, TerminalPostExistsResponse postExistsResponse, ShiftService shiftService)
    {
        _manager = manager;

        _postExistsResponse = postExistsResponse;

        _start = postExistsResponse.Posts.OpenDate;
        _shiftService = shiftService;

        _isStarted = new(postExistsResponse.Posts.IsOpen);

        IsStarted = new(_isStarted);
        _terminalPostApi = terminalPostApi;
        _reportShiftService = reportShiftService;
    }

    public MemberDto Manager => _manager;

    public ObservableOnlyBehaviourSubject<bool> IsStarted
    {
        get;
    }

    public async Task Start()
    {
        var shift = CreateDto();

        var openShiftRequest = new TerminalOpenPostRequest(DateTime.Now, shift.Id, 0);

        await _terminalPostApi.OpenPost(openShiftRequest);

        _isStarted.OnNext(true);
    }

    public async Task End()
    {
        var shift = CreateDto();

        var closeShiftRequest = new TerminalClosePostRequest(DateTime.Now, shift.Id);

        await _terminalPostApi.ClosePost(closeShiftRequest);

        if (_shiftService.CurrentShift.Value is EdgarShift edgarShift)
        {
            await edgarShift.End();
        }

        //TODO: Add report shift
        _reportShiftService.AddCurrentReportShift(new ReportShiftDto()
        {
        });

        _shiftService._currentCashierShift.OnNext(null);
        _shiftService._currentShift.OnNext(null);
    }

    public CashierShiftDto CreateDto()
    {
        _cashierShift ??= new CashierShiftDto()
        {
            Id = _postExistsResponse.PostId,
            IsStarted = _postExistsResponse.Posts.IsOpen,
            MemberId = _manager.Id,
            Start = _start,
            Number = _postExistsResponse.Posts.Number,
            ManagerId = _postExistsResponse.Posts.ManagerId ?? Guid.Empty,
        };

        return _cashierShift;
    }
}
