using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public sealed class CashierShiftReportPageVm : PageViewModel
{

    public CashierShiftReportPageVm(IReportShiftService reportShift, ReportShiftDto reportShiftDto)
    {
        ReportShiftDto = reportShiftDto;

        ClosePageCommand = ReactiveCommand.Create(() =>
        {
            reportShift.ClearCurrentReportShift();
        });
    }


    public ReportShiftDto ReportShiftDto
    {
        get;
    }

    /// <summary>
    /// Only to subscribe to the event!!!
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClosePageCommand
    {
        get;
    }
}
