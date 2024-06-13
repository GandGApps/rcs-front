using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class ReportShiftService : IReportShiftService
{
    private readonly AdapterBehaviorSubject<ReportShiftDto?> _currentReportShift = new(null);

    public IObservableOnlyBehaviourSubject<ReportShiftDto?> CurrentReportShift => _currentReportShift;

    public void AddCurrentReportShift(ReportShiftDto reportShiftDto)
    {
        _currentReportShift.OnNext(reportShiftDto);
    }

    public void ClearCurrentReportShift()
    {
        _currentReportShift.OnNext(null);
    }
}
