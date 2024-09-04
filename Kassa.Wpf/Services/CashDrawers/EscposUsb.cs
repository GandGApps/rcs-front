using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared.ServiceLocator;
using Kassa.Wpf.Services.PosPrinters;
using Splat;

namespace Kassa.Wpf.Services.CashDrawers;
internal sealed class EscposUsb : ICashDrawer, IEnableLogger
{
    public Task Open()
    {

        var printerImplementation = RcsLocator.GetService<IPrinter>();

        // Simple way to test if the printer is EscPosUsbPrinter
        if (printerImplementation is not EscPosUsbPrinter usbPrinter)
        {
            this.Log().Error("Printer is not EscPosUsbPrinter");

            return Task.CompletedTask;
        }

        // And also simple way to get the printer name
        var printer = EscPosUsbPrinter.GetPrinter(usbPrinter._printerName);

        if (printer == null)
        {
            this.Log().Error("Printer is null");

            return Task.CompletedTask;
        }

        printer.OpenDrawer();
        printer.PrintDocument();

        return Task.CompletedTask;
    }
}
