using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Microsoft.Extensions.Options;
using Splat;

namespace Kassa.Wpf.Services;
public sealed class PosPrinterSelector : IEnableLogger, IPrinter
{
    private IPrinter _printer;

    public PosPrinterSelector(IOptionsMonitor<PrinterPosLib> printerPosLib)
    {

        printerPosLib.OnChange((lib) =>
        {
            this.Log().Info($"PrinterPosLib changed to {lib}");

            Volatile.Write(ref _printer, lib switch
            {
                PrinterPosLib.Wnd => new WndPrinter(false),
                PrinterPosLib.Wndpos => new WndPosPrinter(),
                _ => throw new NotImplementedException()
            });
        });
    }

    public Task PrintAsync(ReportShiftDto reportShift) => _printer.PrintAsync(reportShift);
    public Task PrintAsync(OrderDto order) => _printer.PrintAsync(order);
}
