using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Wpf.Services.PosPrinters;
using Splat;

namespace Kassa.Wpf.Services.CashDrawers;
internal sealed class EscposUsb : ICashDrawer, IEnableLogger
{
    public Task Open()
    {

        var printerImplementation = Locator.Current.GetService<IPrinter>();

        if (printerImplementation is not EscPosUsbPrinter usbPrinter)
        {
            this.Log().Error("Printer is not EscPosUsbPrinter");

            return Task.CompletedTask;
        }

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
