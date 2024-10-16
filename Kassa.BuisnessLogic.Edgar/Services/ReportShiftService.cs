using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class ReportShiftService : IReportShiftService, IEnableLogger
{
    private readonly AdapterBehaviorSubject<ReportShiftDto?> _currentReportShift = new(null);
    private readonly IPrinter? _printer;

    public ReportShiftService(IPrinter? printer)
    {
        _printer = printer;
    }

    public IObservableOnlyBehaviourSubject<ReportShiftDto?> CurrentReportShift => _currentReportShift;

    public void AddCurrentReportShift(ReportShiftDto reportShiftDto)
    {
        _currentReportShift.OnNext(reportShiftDto);
    }

    public void ClearCurrentReportShift()
    {
        var currentReportShift = _currentReportShift.Value;

        if (currentReportShift == null)
        {
            return;
        }

        _currentReportShift.OnNext(null);

        if (_printer is null)
        {
            this.Log().Warn($"{nameof(IPrinter)} is not registered");
            return;
        }

        _printer.PrintAsync(currentReportShift);
    }
}
