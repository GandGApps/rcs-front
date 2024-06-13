using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IReportShiftService
{
    public IObservableOnlyBehaviourSubject<ReportShiftDto?> CurrentReportShift
    {
        get;
    }

    public void AddCurrentReportShift(ReportShiftDto reportShiftDto);
    public void ClearCurrentReportShift();
}
